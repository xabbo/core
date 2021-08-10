using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Messages;
using Xabbo.Interceptor;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game
{
    public class FriendManager : GameStateManager
    {
        private readonly ILogger _logger;

        private readonly ConcurrentDictionary<long, Friend> _friends = new();
        private readonly ConcurrentDictionary<string, Friend> _nameMap = new(StringComparer.OrdinalIgnoreCase);

        private int _totalFragments = -1, _currentFragment = 0;
        private readonly List<Friend> _loadList = new();
        private bool _isLoading = true, _isForceLoading;

        public bool IsInitialized { get; private set; }
        public IEnumerable<IFriend> Friends => _friends.Select(x => x.Value);
        public bool IsFriend(long id) => _friends.ContainsKey(id);
        public bool IsFriend(string name) => _nameMap.ContainsKey(name);
        public IFriend? GetFriend(long id) => _friends.TryGetValue(id, out Friend? friend) ? friend : null;
        public IFriend? GetFriend(string name) => _nameMap.TryGetValue(name, out Friend? friend) ? friend : null;

        #region - Events -
        public event EventHandler? Loaded;
        protected virtual void OnLoaded()
            => Loaded?.Invoke(this, EventArgs.Empty);

        public event EventHandler<FriendEventArgs>? FriendAdded;
        protected virtual void OnFriendAdded(IFriend friend)
            => FriendAdded?.Invoke(this, new FriendEventArgs(friend));

        public event EventHandler<FriendEventArgs>? FriendRemoved;
        protected virtual void OnFriendRemoved(IFriend friend)
            => FriendRemoved?.Invoke(this, new FriendEventArgs(friend));

        public event EventHandler<FriendUpdatedEventArgs>? FriendUpdated;
        protected virtual void OnFriendUpdated(IFriend previous, IFriend current)
            => FriendUpdated?.Invoke(this, new FriendUpdatedEventArgs(previous, current));

        public event EventHandler<FriendMessageEventArgs>? MessageReceived;
        protected virtual void OnMessageReceived(Friend friend, string message)
            => MessageReceived?.Invoke(this, new FriendMessageEventArgs(this, friend, message));
        #endregion

        public FriendManager(IInterceptor interceptor,
            ILogger<FriendManager> logger)
            : base(interceptor)
        {
            _logger = logger;
        }

        public FriendManager(IInterceptor interceptor)
            : base(interceptor)
        {
            _logger = NullLogger.Instance;
        }

        protected override void OnDisconnected(object? sender, EventArgs e)
        {
            base.OnDisconnected(sender, e);

            _friends.Clear();
            _nameMap.Clear();

            _totalFragments = -1;
            _currentFragment = 0;
            _loadList.Clear();
            _isLoading = true;
            _isForceLoading = false;
        }

        public void SendMessage(long id, string message) => SendAsync(Out.SendMessage, id, message);
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

        private void RemoveFriend(long id)
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

        [InterceptIn(nameof(Incoming.ClientLatencyPingResponse))]
        private async void HandleClientLatencyPingResponse(InterceptArgs e)
        {
            if (!IsInitialized && !_isForceLoading && e.Step > 50)
            {
                _isForceLoading = true;
                await SendAsync(Out.MessengerInit);
            }
        }

        [InterceptIn(nameof(Incoming.MessengerInit))]
        protected virtual void HandleMessengerInit(InterceptArgs e)
        {
            if (_isLoading && _isForceLoading)
                e.Block();
        }

        [InterceptIn(nameof(Incoming.FriendListFragment))]
        protected virtual void HandleFriendListFragment(InterceptArgs e)
        {
            if (!_isLoading)
                return;

            int total = e.Packet.ReadInt();
            int current = e.Packet.ReadInt();

            if (current != _currentFragment) return;
            if (_totalFragments == -1) _totalFragments = total;
            else if (_totalFragments != total) return;
            _currentFragment++;

            if (_isForceLoading)
                e.Block();

            short n = e.Packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
                _loadList.Add(Friend.Parse(e.Packet));

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

        [InterceptIn(nameof(Incoming.FriendListUpdate))]
        protected virtual void HandleFriendListUpdate(InterceptArgs e)
        {
            short n = e.Packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
            {
                e.Packet.ReadInt(); // category id : -1 = offline, 0 = online
                e.Packet.ReadString(); // category name
            }

            n = e.Packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
            {
                int updateType = e.Packet.ReadInt();
                if (updateType == -1) // removed
                {
                    long id = e.Packet.ReadLegacyLong();
                    RemoveFriend(id);
                }
                else if (updateType == 0) // updated
                {
                    Friend friend = Friend.Parse(e.Packet);
                    UpdateFriend(friend);
                }
                else if (updateType == 1) // added
                {
                    Friend friend = Friend.Parse(e.Packet);
                    AddFriend(friend);
                }
            }
        }

        [InterceptIn(nameof(Incoming.MessengerNewConsoleMessage))]
        protected virtual void OnReceivePrivateMessage(InterceptArgs e)
        {
            long id = e.Packet.ReadLong();
            if (!_friends.TryGetValue(id, out Friend? friend))
            {
                DebugUtil.Log($"failed to get friend {friend} from id map");
                return;
            }

            string message = e.Packet.ReadString();
            // int secondsSinceSent
            // string extraData

            OnMessageReceived(friend, message);
        }
    }
}
