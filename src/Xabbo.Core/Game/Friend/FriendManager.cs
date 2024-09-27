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

/// <summary>
/// Manages the user's friend list.
/// </summary>
[Intercept]
public sealed partial class FriendManager(IInterceptor interceptor, ILoggerFactory? loggerFactory = null) : GameStateManager(interceptor)
{
    private readonly ILogger Log = (ILogger?)loggerFactory?.CreateLogger<FriendManager>() ?? NullLogger.Instance;

    private readonly ConcurrentDictionary<Id, Friend> _friends = new();
    private readonly ConcurrentDictionary<string, Friend> _nameMap = new(StringComparer.OrdinalIgnoreCase);

    private int _totalFragments = -1, _currentFragment = 0;
    private readonly List<Friend> _loadList = [];
    private bool _isLoading = true, _isForceLoading, _receivedMessengerInit;

    /// <summary>
    /// Gets whether the user's friend list has been initialized.
    /// </summary>
    public bool IsInitialized { get; private set; }

    /// <summary>
    /// Gets the user's friends.
    /// </summary>
    public IEnumerable<IFriend> Friends => _friends.Select(x => x.Value);

    /// <summary>
    /// Gets whether a user with the specified user ID is in the current user's friend list.
    /// </summary>
    /// <param name="userId">The ID of the user to check.</param>
    public bool IsFriend(Id userId) => _friends.ContainsKey(userId);

    /// <summary>
    /// Gets whether a user with the specified name is in the current user's friend list.
    /// </summary>
    /// <param name="name">The name of the user to check. Case-insensitive.</param>
    public bool IsFriend(string name) => _nameMap.ContainsKey(name);

    /// <summary>
    /// Gets the friend with the specified ID, or returns <c>null</c> if the friend does not exist.
    /// </summary>
    /// <param name="userId">The ID of the friend to get.</param>
    public IFriend? GetFriend(Id userId) => _friends.TryGetValue(userId, out Friend? friend) ? friend : null;

    /// <summary>
    /// Gets the friend with the specified name, or returns <c>null</c> if the friend does not exist.
    /// The name is case-insensitive.
    /// </summary>
    /// <param name="name">The name of the friend to get. Case-insensitive.</param>
    public IFriend? GetFriend(string name) => _nameMap.TryGetValue(name, out Friend? friend) ? friend : null;

    #region - Events -
    /// <summary>
    /// Invoked when the user's friends are loaded.
    /// </summary>
    public event Action? Loaded;

    /// <summary>
    /// Invoked when a friend is added.
    /// </summary>
    public event Action<FriendEventArgs>? FriendAdded;

    /// <summary>
    /// Invoked when a friend is removed.
    /// </summary>
    public event Action<FriendEventArgs>? FriendRemoved;

    /// <summary>
    /// Invoked when a friend is updated.
    /// </summary>
    public event Action<FriendUpdatedEventArgs>? FriendUpdated;

    /// <summary>
    /// Invoked when a message is received from a friend.
    /// </summary>
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
    public void SendMessage(Id userId, string message) => Interceptor.Send(new SendConsoleMessageMsg { Recipients = [userId], Message = message });

    /// <summary>
    /// Sends a private message to the specified friend.
    /// </summary>
    public void SendMessage(IFriend friend, string message) => SendMessage(friend.Id, message);

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
