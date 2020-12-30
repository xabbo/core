using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    public class FriendManager : XabboComponent
    {
        private int totalExpected = -1, currentIndex = 0;
        private readonly List<Friend> loadList = new List<Friend>();
        private bool isLoadingFriends = true, isForceLoading;

        private readonly ConcurrentDictionary<int, Friend> friends;
        private readonly ConcurrentDictionary<string, Friend> nameMap;

        public bool IsInitialized { get; private set; }
        public IEnumerable<IFriend> Friends => friends.Select(x => x.Value);
        public bool IsFriend(int id) => friends.ContainsKey(id);
        public bool IsFriend(string name) => nameMap.ContainsKey(name.ToLower());
        public IFriend GetFriend(int id) => friends.TryGetValue(id, out Friend friend) ? friend : null;
        public IFriend GetFriend(string name) => nameMap.TryGetValue(name.ToLower(), out Friend friend) ? friend : null;

        #region - Events -
        public event EventHandler Loaded;
        protected virtual void OnLoaded()
            => Loaded?.Invoke(this, EventArgs.Empty);

        public event EventHandler<FriendEventArgs> FriendAdded;
        protected virtual void OnFriendAdded(IFriend friend)
            => FriendAdded?.Invoke(this, new FriendEventArgs(friend));
        
        public event EventHandler<FriendEventArgs> FriendRemoved;
        protected virtual void OnFriendRemoved(IFriend friend)
            => FriendRemoved?.Invoke(this, new FriendEventArgs(friend));

        public event EventHandler<FriendUpdatedEventArgs> FriendUpdated;
        protected virtual void OnFriendUpdated(IFriend previous, IFriend current)
            => FriendUpdated?.Invoke(this, new FriendUpdatedEventArgs(previous, current));

        public event EventHandler<FriendMessageEventArgs> MessageReceived;
        protected virtual void OnMessageReceived(Friend friend, string message)
            => MessageReceived?.Invoke(this, new FriendMessageEventArgs(this, friend, message));
        #endregion

        public FriendManager()
        {
            friends = new ConcurrentDictionary<int, Friend>();
            nameMap = new ConcurrentDictionary<string, Friend>(StringComparer.OrdinalIgnoreCase);
        }

        protected override void OnInitialize() { }

        public void SendMessage(int id, string message) => SendAsync(Out.FriendPrivateMessage, id, message);
        public void SendMessage(Friend friend, string message) => SendMessage(friend.Id, message);

        private void AddFriend(Friend friend, bool raiseEvent = true)
        {
            if (friends.TryAdd(friend.Id, friend))
            {
                if (!nameMap.TryAdd(friend.Name, friend))
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
            if (friends.TryGetValue(friend.Id, out Friend previous) &&
                friends.TryUpdate(friend.Id, friend, previous))
            {
                if (!nameMap.TryRemove(previous.Name, out _) ||
                    !nameMap.TryAdd(friend.Name, friend))
                {
                    DebugUtil.Log($"failed to update friend {friend} in name map");
                }

                if (raiseEvent)
                    OnFriendUpdated(previous, friend);
            }
            else
            {
                DebugUtil.Log($"failed to get friend {friend} from id map to update");
            }
        }

        private void RemoveFriend(int id)
        {
            if (friends.TryRemove(id, out Friend friend))
            {
                if (!nameMap.TryRemove(friend.Name, out _))
                    DebugUtil.Log("failed to remove friend from name map");

                OnFriendRemoved(friend);
            }
            else
            {
                DebugUtil.Log("failed to remove friend from id mape");
            }
        }

        private void RemoveFriend(Friend friend) => RemoveFriend(friend.Id);

        [Receive("LatencyResponse"), RequiredOut("RequestInitFriends")]
        private async void HandleLatencyResponse(IReadOnlyPacket packet)
        {
            if (!IsInitialized && !isForceLoading)
            {
                DebugUtil.Log("force loading friends");

                isForceLoading = true;
                await SendAsync(Out.RequestInitFriends);
            }
        }

        [InterceptIn(nameof(Incoming.InitFriends))]
        protected virtual void HandleInitFriends(InterceptArgs e)
        {
            if (isLoadingFriends && isForceLoading)
                e.Block();
        }

        [InterceptIn(nameof(Incoming.Friends))]
        protected virtual void HandleFriends(InterceptArgs e)
        {
            if (!isLoadingFriends)
                return;

            int total = e.Packet.ReadInt();
            int current = e.Packet.ReadInt();

            if (current != currentIndex) return;
            if (totalExpected == -1) totalExpected = total;
            else if (totalExpected != total) return;
            currentIndex++;

            if (isForceLoading)
                e.Block();

            int n = e.Packet.ReadInt();
            for (int i = 0; i < n; i++)
                loadList.Add(Friend.Parse(e.Packet));

            if (currentIndex == total)
            {
                AddFriends(loadList, false);

                isLoadingFriends = false;
                isForceLoading = false;
                IsInitialized = true;
                OnLoaded();
            }
        }

        [InterceptIn(nameof(Incoming.UpdateFriend))]
        protected virtual void OnUpdateFriend(InterceptArgs e)
        {
            int n = e.Packet.ReadInt();
            for (int i = 0; i < n; i++)
            {
                e.Packet.ReadInt(); // -1 = offline, 0 = online
                e.Packet.ReadString(); // group name
            }

            n = e.Packet.ReadInt();
            for (int i = 0; i < n; i++)
            {
                int updateType = e.Packet.ReadInt();
                if (updateType == -1) // removed
                {
                    int id = e.Packet.ReadInt();
                    RemoveFriend(id);
                }
                else if (updateType == 0) // updated
                {
                    var friend = Friend.Parse(e.Packet);
                    UpdateFriend(friend);
                }
                else if (updateType == 1) // added
                {
                    var friend = Friend.Parse(e.Packet);
                    AddFriend(friend);
                }
            }
        }

        [InterceptIn(nameof(Incoming.ReceivePrivateMessage))]
        protected virtual void OnReceivePrivateMessage(InterceptArgs e)
        {
            int id = e.Packet.ReadInt();
            if (!friends.TryGetValue(id, out Friend friend))
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
