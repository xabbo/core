using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Interceptor;
using Xabbo.Messages.Flash;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

[Intercept]
public sealed partial class FriendManager : GameStateManager
{
    private readonly ILogger _logger;

    private readonly ConcurrentDictionary<Id, Friend> _friends = new();
    private readonly ConcurrentDictionary<string, Friend> _nameMap = new(StringComparer.OrdinalIgnoreCase);

    private int _totalFragments = -1, _currentFragment = 0;
    private readonly List<Friend> _loadList = [];
    private bool _isLoading = true, _isForceLoading;

    public bool IsInitialized { get; private set; }
    public IEnumerable<IFriend> Friends => _friends.Select(x => x.Value);
    public bool IsFriend(Id id) => _friends.ContainsKey(id);
    public bool IsFriend(string name) => _nameMap.ContainsKey(name);
    public IFriend? GetFriend(Id id) => _friends.TryGetValue(id, out Friend? friend) ? friend : null;
    public IFriend? GetFriend(string name) => _nameMap.TryGetValue(name, out Friend? friend) ? friend : null;

    #region - Events -
    public event Action? Loaded;
    private void OnLoaded() => Loaded?.Invoke();

    public event Action<FriendEventArgs>? FriendAdded;
    private void OnFriendAdded(IFriend friend) => FriendAdded?.Invoke(new FriendEventArgs(friend));

    public event Action<FriendEventArgs>? FriendRemoved;
    private void OnFriendRemoved(IFriend friend) => FriendRemoved?.Invoke(new FriendEventArgs(friend));

    public event Action<FriendUpdatedEventArgs>? FriendUpdated;
    private void OnFriendUpdated(IFriend previous, IFriend current) => FriendUpdated?.Invoke(new FriendUpdatedEventArgs(previous, current));

    public event Action<FriendMessageEventArgs>? MessageReceived;
    private void OnMessageReceived(Friend friend, string message) => MessageReceived?.Invoke(new FriendMessageEventArgs(friend, message));
    #endregion

    public FriendManager(IInterceptor interceptor, ILogger<FriendManager> logger)
        : base(interceptor)
    {
        _logger = logger;
    }

    public FriendManager(IInterceptor interceptor)
        : base(interceptor)
    {
        _logger = NullLogger.Instance;
    }

    protected override void OnDisconnected()
    {
        _friends.Clear();
        _nameMap.Clear();

        _totalFragments = -1;
        _currentFragment = 0;
        _loadList.Clear();
        _isLoading = true;
        _isForceLoading = false;
    }

    /// <summary>
    /// Sends a private message to a user with the specified ID.
    /// </summary>
    public void SendMessage(Id id, string message) => Interceptor.Send(Out.PostMessage, id, message);

    /// <summary>
    /// Sends a private message to the specified friend.
    /// </summary>
    public void SendMessage(Friend friend, string message) => SendMessage(friend.Id, message);

    private void AddFriend(Friend friend, bool raiseEvent = true)
    {
        if (_friends.TryAdd(friend.Id, friend))
        {
            if (!_nameMap.TryAdd(friend.Name, friend))
                _logger.LogError("Failed to add friend {friend} to name map", friend);

            if (raiseEvent)
            {
                _logger.LogTrace("Added friend {friend}", friend);
                OnFriendAdded(friend);
            }
        }
        else
        {
            _logger.LogError("Failed to add friend {friend} to id map", friend);
        }
    }

    private void AddFriends(IEnumerable<Friend> friends, bool raiseEvent = true)
    {
        foreach (var friend in friends)
            AddFriend(friend, raiseEvent);
    }

    private void UpdateFriend(Friend friend)
    {
        if (_friends.TryGetValue(friend.Id, out Friend? previous) &&
            _friends.TryUpdate(friend.Id, friend, previous))
        {
            if (previous.Name != friend.Name)
            {
                if (!_nameMap.TryRemove(previous.Name, out _) ||
                    !_nameMap.TryAdd(friend.Name, friend))
                {
                    _logger.LogError("Failed to update friend {friend} in name map", friend);
                }
            }

            _logger.LogTrace("Updated friend {friend}", friend);
            OnFriendUpdated(previous, friend);
        }
        else
        {
            _logger.LogError("Failed to get friend {friend} from id map to update", friend);
        }
    }

    private void RemoveFriend(Id id)
    {
        if (_friends.TryRemove(id, out Friend? friend))
        {
            if (!_nameMap.TryRemove(friend.Name, out _))
                _logger.LogError("Failed to remove friend {id} from name map", id);

            _logger.LogTrace("Removed friend {friend}", friend);
            OnFriendRemoved(friend);
        }
        else
        {
            _logger.LogError("Failed to remove friend {id} from id map", id);
        }
    }

    [InterceptIn(nameof(In.LatencyPingResponse))]
    private void HandleLatencyPingResponse(Intercept e)
    {
        if (!IsInitialized && !_isForceLoading && e.Sequence > 10)
        {
            _isForceLoading = true;
            Interceptor.Send(Out.MessengerInit);
        }
    }

    [InterceptIn(nameof(In.MessengerInit))]
    private void HandleMessengerInit(Intercept e)
    {
        if (_isLoading && _isForceLoading)
            e.Block();
    }

    [InterceptIn(nameof(In.FriendListFragment))]
    private void HandleFriendListFragment(Intercept e)
    {
        if (!_isLoading)
            return;

        int total = e.Packet.Read<int>();
        int current = e.Packet.Read<int>();

        if (current != _currentFragment) return;
        if (_totalFragments == -1) _totalFragments = total;
        else if (_totalFragments != total) return;
        _currentFragment++;

        if (_isForceLoading)
            e.Block();

        int n = e.Packet.Read<Length>();
        for (int i = 0; i < n; i++)
            _loadList.Add(e.Packet.Read<Friend>());

        if (_currentFragment == total)
        {
            AddFriends(_loadList, false);

            _isLoading = false;
            _isForceLoading = false;
            IsInitialized = true;
            OnLoaded();
        }

        _logger.LogTrace(
            "Received friend list fragment {fragmentIndex}/{totalFragments} ({fragmentCount})",
            _currentFragment, total, n
        );
    }

    [InterceptIn(nameof(In.FriendListUpdate))]
    private void HandleFriendListUpdate(Intercept e)
    {
        int n = e.Packet.Read<Length>();
        for (int i = 0; i < n; i++)
        {
            e.Packet.Read<int>(); // category id : -1 = offline, 0 = online
            e.Packet.Read<string>(); // category name
        }

        n = e.Packet.Read<Length>();
        for (int i = 0; i < n; i++)
        {
            int updateType = e.Packet.Read<int>();
            if (updateType == -1) // removed
            {
                long id = e.Packet.Read<Id>();
                RemoveFriend(id);
            }
            else if (updateType == 0) // updated
            {
                Friend friend = e.Packet.Read<Friend>();
                UpdateFriend(friend);
            }
            else if (updateType == 1) // added
            {
                Friend friend = e.Packet.Read<Friend>();
                AddFriend(friend);
            }
        }
    }

    [InterceptIn(nameof(In.NewConsole))]
    private void HandleNewConsole(Intercept e)
    {
        Id id = e.Packet.Read<Id>();
        if (!_friends.TryGetValue(id, out Friend? friend))
        {
            Debug.Log($"failed to get friend {friend} from id map");
            return;
        }

        string message = e.Packet.Read<string>();
        // int secondsSinceSent
        // string extraData

        OnMessageReceived(friend, message);
    }
}
