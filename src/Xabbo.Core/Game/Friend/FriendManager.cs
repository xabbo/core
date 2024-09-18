using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Interceptor;
using Xabbo.Messages.Flash;

using Xabbo.Core.Events;
using Xabbo.Core.Messages.Outgoing;

namespace Xabbo.Core.Game;

[Intercept]
public sealed partial class FriendManager(IInterceptor interceptor, ILoggerFactory? loggerFactory = null) : GameStateManager(interceptor)
{
    private readonly ILogger Log = (ILogger?)loggerFactory?.CreateLogger<FriendManager>() ?? NullLogger.Instance;

    private readonly ConcurrentDictionary<Id, Friend> _friends = new();
    private readonly ConcurrentDictionary<string, Friend> _nameMap = new(StringComparer.OrdinalIgnoreCase);

    private int _totalFragments = -1, _currentFragment = 0;
    private readonly List<Friend> _loadList = [];
    private bool _isLoading = true, _isForceLoading, _receivedMessengerInit;

    public bool IsInitialized { get; private set; }
    public IEnumerable<IFriend> Friends => _friends.Select(x => x.Value);
    public bool IsFriend(Id id) => _friends.ContainsKey(id);
    public bool IsFriend(string name) => _nameMap.ContainsKey(name);
    public IFriend? GetFriend(Id id) => _friends.TryGetValue(id, out Friend? friend) ? friend : null;
    public IFriend? GetFriend(string name) => _nameMap.TryGetValue(name, out Friend? friend) ? friend : null;

    #region - Events -
    public event Action? Loaded;

    public event Action<FriendEventArgs>? FriendAdded;

    public event Action<FriendEventArgs>? FriendRemoved;

    public event Action<FriendUpdatedEventArgs>? FriendUpdated;

    public event Action<FriendMessageEventArgs>? MessageReceived;
    #endregion

    protected override void OnConnected(GameConnectedArgs e)
    {
        base.OnConnected(e);
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
        _receivedMessengerInit = false;
    }

    /// <summary>
    /// Sends a private message to a user with the specified ID.
    /// </summary>
    public void SendMessage(Id id, string message) => Interceptor.Send(new SendConsoleMessageMsg { Recipients = [id], Message = message });

    /// <summary>
    /// Sends a private message to the specified friend.
    /// </summary>
    public void SendMessage(Friend friend, string message) => SendMessage(friend.Id, message);

    protected override void OnInitialize(bool initializingOnConnect)
    {
        if (!_receivedMessengerInit)
        {
            _isForceLoading = true;
            Interceptor.Send(Out.MessengerInit);
        }
    }

    private void CompleteLoadingFriends()
    {
        Log.LogInformation("Loaded {Count} friends.", _friends.Count);

        _isLoading = false;
        _isForceLoading = false;
        _loadList.Clear();

        IsInitialized = true;
        Loaded?.Invoke();
    }

    private void AddFriend(Friend friend)
    {
        if (!_friends.TryAdd(friend.Id, friend))
        {
            Log.LogWarning("Failed to add friend #{Id} '{Name}'.", friend.Id, friend.Name);
            return;
        }

        if (_isLoading)
        {
            Log.LogTrace("Loaded friend #{Id} '{Name}'.", friend.Id, friend.Name);
        }
        else
        {
            Log.LogInformation("Added friend #{Id} '{Name}'.", friend.Id, friend.Name);
            FriendAdded?.Invoke(new FriendEventArgs(friend));
        }

        if (!_nameMap.TryAdd(friend.Name, friend))
        {
            Log.LogWarning("Failed to add friend #{Id} '{Name}' to name map.", friend.Id, friend.Name);
        }
    }

    private void AddFriends(IEnumerable<Friend> friends)
    {
        foreach (var friend in friends)
            AddFriend(friend);
    }

    private void UpdateFriend(Friend friend)
    {
        if (!_friends.TryGetValue(friend.Id, out Friend? previous) ||
            !_friends.TryUpdate(friend.Id, friend, previous))
        {
            Log.LogWarning("Failed to get friend #{Id} '{Name}' from ID map to update.", friend.Id, friend.Name);
            return;
        }

        Log.LogTrace("Updated friend #{Id} '{Name}'.", friend.Id, friend.Name);

        if (previous.Name != friend.Name)
        {
            if (!_nameMap.TryRemove(previous.Name, out _) ||
                !_nameMap.TryAdd(friend.Name, friend))
            {
                Log.LogWarning("Failed to update friend #{Id} '{Name}' in name map.", friend.Id, friend.Name);
            }
        }

        FriendUpdated?.Invoke(new FriendUpdatedEventArgs(previous, friend));
    }

    private void RemoveFriend(Id id)
    {
        if (!_friends.TryRemove(id, out Friend? friend))
        {
            Log.LogWarning("Failed to remove friend #{Id} from ID map.", id);
            return;
        }

        if (!_nameMap.TryRemove(friend.Name, out _))
            Log.LogWarning("Failed to remove friend #{Id} '{Name}' from name map.", id, friend.Name);

        Log.LogInformation("Removed friend #{Id} '{Name}'.", friend.Id, friend.Name);
        FriendRemoved?.Invoke(new FriendEventArgs(friend));
    }
}
