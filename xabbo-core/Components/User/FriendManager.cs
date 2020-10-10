using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    public class FriendManager : XabboComponent
    {
        public enum Feature { Autoload }

        private int totalExpected = -1, currentIndex = 0;
        private readonly List<FriendInfo> loadList = new List<FriendInfo>();
        private bool isLoadingFriends = true, isForceLoading;

        private ConcurrentDictionary<int, FriendInfo> friends;
        private ConcurrentDictionary<string, FriendInfo> nameMap;

        public bool IsInitialized { get; private set; }
        public IEnumerable<IFriendInfo> Friends => friends.Select(x => x.Value);
        public bool IsFriend(int id) => friends.ContainsKey(id);
        public bool IsFriend(string name) => nameMap.ContainsKey(name.ToLower());
        public IFriendInfo GetFriend(int id) => friends.TryGetValue(id, out FriendInfo friend) ? friend : null;
        public IFriendInfo GetFriend(string name) => nameMap.TryGetValue(name.ToLower(), out FriendInfo friend) ? friend : null;

        #region - Events -
        public event EventHandler Loaded;
        protected virtual void OnLoaded() => Loaded?.Invoke(this, EventArgs.Empty);

        public EventHandler<FriendEventArgs> FriendAdded;
        protected virtual void OnFriendAdded(FriendInfo friend) => FriendAdded?.Invoke(this, new FriendEventArgs(friend));
        
        public EventHandler<FriendEventArgs> FriendRemoved;
        protected virtual void OnFriendRemoved(FriendInfo friend) => FriendRemoved?.Invoke(this, new FriendEventArgs(friend));

        public EventHandler<FriendEventArgs> FriendUpdated; // TODO FriendUpdatedEventArgs ?
        protected virtual void OnFriendUpdated(FriendInfo friend) => FriendUpdated?.Invoke(this, new FriendEventArgs(friend));
        #endregion

        public FriendManager()
        {
            friends = new ConcurrentDictionary<int, FriendInfo>();
            nameMap = new ConcurrentDictionary<string, FriendInfo>();
        }

        protected override void OnInitialize() { }

        private void AddFriend(FriendInfo friend, bool raiseEvent = true)
        {
            if (friends.TryAdd(friend.Id, friend))
            {
                if (!nameMap.TryAdd(friend.Name.ToLower(), friend))
                    DebugUtil.Log("failed to add friend to name map");

                if (raiseEvent)
                    OnFriendAdded(friend);
            }
            else
            {
                DebugUtil.Log("failed to add friend to id map");
            }
        }

        private void AddFriends(IEnumerable<FriendInfo> friends, bool raiseEvent = true)
        {
            foreach (var friend in friends)
                AddFriend(friend, raiseEvent);
        }

        private void RemoveFriend(FriendInfo friend)
        {
            if (friends.TryRemove(friend.Id, out _))
            {
                if (!nameMap.TryRemove(friend.Name.ToLower(), out _))
                    DebugUtil.Log("failed to remove friend from name map");

                OnFriendRemoved(friend);
            }
            else
            {
                DebugUtil.Log("failed to remove friend from id mape");
            }
        }

        [Group(Feature.Autoload), Receive("LatencyResponse"), RequiredOut("RequestInitFriends")]
        private async void HandleLatencyResponse(Packet packet)
        {
            if (!Dispatcher.IsAttached(this, MessageGroups.Default)) return;

            if (!IsInitialized && !isForceLoading)
            {
                DebugUtil.Log("force loading friends");

                isForceLoading = true;
                await SendAsync(Out.RequestInitFriends);
            }
        }

        [InterceptIn("InitFriends")]
        protected virtual void HandleInitFriends(InterceptEventArgs e)
        {
            if (isLoadingFriends && isForceLoading)
                e.Block();
        }

        [InterceptIn("Friends")]
        protected virtual void HandleFriends(InterceptEventArgs e)
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
                loadList.Add(FriendInfo.Parse(e.Packet));

            if (currentIndex == total)
            {
                AddFriends(loadList, false);

                isLoadingFriends = false;
                isForceLoading = false;
                IsInitialized = true;
                OnLoaded();
            }
        }


    }
}
