using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;

namespace Xabbo.Core.Game
{
    public class FriendManager : GameStateManager
    {
        private int _totalFragments = -1, _currentFragment = 0;
        private readonly List<Friend> _loadList = new();
        private bool _isLoading = true, _isForceLoading;

        private readonly ConcurrentDictionary<long, Friend> _friends = new();
        private readonly ConcurrentDictionary<string, Friend> _nameMap = new(StringComparer.OrdinalIgnoreCase);

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

        public FriendManager(IInterceptor interceptor)
            : base(interceptor)
        {

        }

        public void SendMessage(long id, string message) => SendAsync(Out.SendMessage, id, message);
        public void SendMessage(Friend friend, string message) => SendMessage(friend.Id, message);

        private void AddFriend(Friend friend, bool raiseEvent = true)
        {
            if (_friends.TryAdd(friend.Id, friend))
            {
                if (!_nameMap.TryAdd(friend.Name, friend))
                    DebugUtil.Log($"failed to add friend {friend} to name map");

                if (raiseEvent)
                    OnFriendAdded(friend);
            }
            else
            {
                DebugUtil.Log($"failed to add friend {friend} to id map");
            }
        }

        private void AddFriends(IEnumerable<Friend> friends, bool raiseEvent = true)
        {
            foreach (var friend in friends)
                AddFriend(friend, raiseEvent);
        }

        private void UpdateFriend(Friend friend, bool raiseEvent = true)
        {
            if (_friends.TryGetValue(friend.Id, out Friend? previous) &&
                _friends.TryUpdate(friend.Id, friend, previous))
            {
                if (previous.Name != friend.Name)
                {
                    if (!_nameMap.TryRemove(previous.Name, out _) ||
                        !_nameMap.TryAdd(friend.Name, friend))
                    {
                        DebugUtil.Log($"failed to update friend {friend} in name map");
                    }
                }

                if (raiseEvent)
                    OnFriendUpdated(previous, friend);
            }
            else
            {
                DebugUtil.Log($"failed to get friend {friend} from id map to update");
            }
        }

        private void RemoveFriend(long id)
        {
            if (_friends.TryRemove(id, out Friend? friend))
            {
                if (!_nameMap.TryRemove(friend.Name, out _))
                    DebugUtil.Log("failed to remove friend from name map");

                OnFriendRemoved(friend);
            }
            else
            {
                DebugUtil.Log("failed to remove friend from id mape");
            }
        }

        private void RemoveFriend(Friend friend) => RemoveFriend(friend.Id);

        [InterceptIn(nameof(Incoming.ClientLatencyPingResponse))]
        private async void HandleLatencyResponse(InterceptArgs e)
        {
            if (!IsInitialized && !_isForceLoading && e.Step > 50)
            {
                _isForceLoading = true;
                await SendAsync(Out.MessengerInit);
            }
        }

        [InterceptIn(nameof(Incoming.FriendBarEventNotification))]
        protected virtual void HandleInitFriends(InterceptArgs e)
        {
            if (_isLoading && _isForceLoading)
                e.Block();
        }

        [InterceptIn(nameof(Incoming.FriendListFragment))]
        protected virtual void HandleFriends(InterceptArgs e)
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

            short n = e.Packet.ReadShort();
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
        }

        [InterceptIn(nameof(Incoming.FriendListUpdate))] // @Legacy UpdateFriend
        protected virtual void OnUpdateFriend(InterceptArgs e)
        {
            short n = e.Packet.ReadShort();
            for (int i = 0; i < n; i++)
            {
                e.Packet.ReadInt(); // -1 = offline, 0 = online
                e.Packet.ReadString(); // group name
            }

            n = e.Packet.ReadShort();
            for (int i = 0; i < n; i++)
            {
                int updateType = e.Packet.ReadInt();
                if (updateType == -1) // removed
                {
                    long id = e.Packet.ReadLong();
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

        [InterceptIn(nameof(Incoming.MessengerNewConsoleMessage))] // @Legacy ReceivePrivateMessage
        protected virtual void OnReceivePrivateMessage(InterceptArgs e)
        {
            long id = e.Packet.ReadLong();
            if (!_friends.TryGetValue(id, out Friend? friend))
            {
                DebugUtil.Log($"failed to get friend {friend} from id map");
                return;
            }

            string message = e.Packet.ReadString();
            // int:? [string:?]

            OnMessageReceived(friend, message);
        }
    }
}
