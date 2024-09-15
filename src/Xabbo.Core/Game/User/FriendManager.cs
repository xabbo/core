using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Interceptor;
using Xabbo.Messages.Flash;

using Xabbo.Core.Events;
using Xabbo.Core.Messages.Incoming;
using Xabbo.Core.Messages.Outgoing;

using ModernIncoming = Xabbo.Core.Messages.Incoming.Modern;
using OriginsIncoming = Xabbo.Core.Messages.Incoming.Origins;

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
    private void OnLoaded()
    {
        Log.Info("Loaded {Count} friends.", args: [_friends.Count]);
        Loaded?.Invoke();
    }

    public event Action<FriendEventArgs>? FriendAdded;
    private void OnFriendAdded(IFriend friend)
    {
        Log.Info("Added friend '{Name}' (id: {Id}).", args: [friend.Name, friend.Id]);
        FriendAdded?.Invoke(new FriendEventArgs(friend));
    }

    public event Action<FriendEventArgs>? FriendRemoved;
    private void OnFriendRemoved(IFriend friend)
    {
        Log.Info("Removed friend '{Name}' (id: {Id}).", args: [friend.Name, friend.Id]);
        FriendRemoved?.Invoke(new FriendEventArgs(friend));
    }

    public event Action<FriendUpdatedEventArgs>? FriendUpdated;
    private void OnFriendUpdated(IFriend previous, IFriend current)
    {
        Log.Trace("Updated friend '{Name}' (id: {Id}).", args: [current.Name, current.Id]);
        FriendUpdated?.Invoke(new FriendUpdatedEventArgs(previous, current));
    }

    public event Action<FriendMessageEventArgs>? MessageReceived;
    private void OnMessageReceived(Friend friend, ConsoleMessage message)
    {
        Log.Trace("Received message from '{Name}': '{Message}'.", args: [friend.Name, message.Content]);
        MessageReceived?.Invoke(new FriendMessageEventArgs(friend, message));
    }
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

    private void CompleteLoadingFriends()
    {
        _isLoading = false;
        _isForceLoading = false;
        _loadList.Clear();
        IsInitialized = true;
        OnLoaded();
    }

    private void AddFriend(Friend friend, bool raiseEvent = true)
    {
        if (_friends.TryAdd(friend.Id, friend))
        {
            if (!_nameMap.TryAdd(friend.Name, friend))
            {
                Log.Error("Failed to add friend {Friend} to name map.", args: [friend]);
            }

            if (raiseEvent)
            {
                OnFriendAdded(friend);
            }
            else
            {
                Log.Info("Added friend '{Name}' (id: {Id}).", args: [friend.Name, friend.Id]);
            }
        }
        else
        {
            Log.Error("Failed to add friend '{Name}' (id: {Id}) to ID map", args: [friend.Name, friend.Id]);
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
                    Log.Warn("Failed to update friend {Friend} in name map.", args: [friend]);
                }
            }

            OnFriendUpdated(previous, friend);
        }
        else
        {
            Log.Warn("Failed to get friend {Friend} from ID map to update.", args: [friend]);
        }
    }

    private void RemoveFriend(Id id)
    {
        if (_friends.TryRemove(id, out Friend? friend))
        {
            if (!_nameMap.TryRemove(friend.Name, out _))
                Log.Warn("Failed to remove friend {Id} from name map.", args: [id]);

            OnFriendRemoved(friend);
        }
        else
        {
            Log.Warn("Failed to remove friend {Id} from ID map.", args: [id]);
        }
    }

    protected override void OnInitialize(bool initializingOnConnect)
    {
        if (!_receivedMessengerInit)
        {
            _isForceLoading = true;
            Interceptor.Send(Out.MessengerInit);
        }
    }

    [Intercept]
    private void HandleMessengerInit(Intercept<MessengerInitMsg> e)
    {
        _receivedMessengerInit = true;
        if (_isLoading)
        {
            if (_isForceLoading)
                e.Block();

            if (Interceptor.Session.Client.Type is ClientType.Shockwave)
            {
                // Friends list is available here
                AddFriends(e.Msg.Friends, false);
                if (_isLoading)
                    CompleteLoadingFriends();
            }
        }
    }

    [Intercept]
    private void HandleFriendList(Intercept<FriendListMsg> e)
    {
        if (!_isLoading)
            return;

        int total = e.Msg.FragmentCount;
        int current = e.Msg.FragmentIndex;

        if (current != _currentFragment)
        {
            Log.Warn("Fragment index mismatch. (expected: {Expected}, actual: {Actual})",
                args: [_currentFragment, current]);
            return;
        }
        if (_totalFragments == -1) _totalFragments = total;
        else if (_totalFragments != total)
        {
            Log.Warn("Fragment count mismatch. (expected: {Expected}, actual: {Actual})",
                args: [_totalFragments, total]);
            return;
        }

        if (_isForceLoading)
        {
            Log.Trace("Blocking packet");
            e.Block();
        }

        _loadList.AddRange(e.Msg);

        _currentFragment++;
        if (_currentFragment == total)
        {
            AddFriends(_loadList, false);
            CompleteLoadingFriends();
        }

        Log.Trace(
            "Received friend list fragment {FragmentIndex}/{TotalFragments} ({FragmentCount})",
            args: [_currentFragment, total, e.Msg.Count]
        );
    }

    [Intercept]
    private void HandleFriendListUpdate(ModernIncoming.FriendListUpdateMsg msg)
    {
        foreach (var update in msg.Updates)
        {
            switch (update.Type)
            {
                case FriendListUpdateType.Add when update.Friend is not null:
                    AddFriend(update.Friend);
                    break;
                case FriendListUpdateType.Update when update.Friend is not null:
                    UpdateFriend(update.Friend);
                    break;
                case FriendListUpdateType.Remove:
                    RemoveFriend(update.Id);
                    break;
            }
        }
    }

    [Intercept]
    private void HandleFriendAdded(OriginsIncoming.FriendAddedMsg msg) => AddFriend(msg.Friend);

    [Intercept]
    private void HandleFriendsRemoved(OriginsIncoming.FriendsRemovedMsg msg)
    {
        foreach (var id in msg)
            RemoveFriend(id);
    }

    [Intercept]
    private void HandleConsoleMessages(ConsoleMessagesMsg messages)
    {
        foreach (var message in messages)
        {
            if (!_friends.TryGetValue(message.SenderId, out Friend? friend))
            {
                Log.Warn("Failed to get friend {Id} from ID map.", args: [message.SenderId]);
                continue;
            }
            OnMessageReceived(friend, message);
        }
    }
}
