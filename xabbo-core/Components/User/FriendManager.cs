using System;
using System.Collections.Generic;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    public class FriendManager : XabboComponent
    {
        public enum Feature { Autoload }

        private bool isLoadingFriends = true, isForceLoading = false;
        private int currentPacket = 0, totalPackets = -1;
        private readonly List<FriendInfo> loadList = new List<FriendInfo>();

        public bool IsInitialized { get; private set; }

        private readonly object listLock = new object();
        public IReadOnlyList<FriendInfo> Friends { get; private set; }

        public event EventHandler Loaded;
        protected virtual void OnLoaded() => Loaded?.Invoke(this, EventArgs.Empty);

        public EventHandler<FriendEventArgs> FriendAdded;
        protected virtual void OnFriendAdded(FriendInfo friend) => FriendAdded?.Invoke(this, new FriendEventArgs(friend));
        
        public EventHandler<FriendEventArgs> FriendRemoved;
        protected virtual void OnFriendRemoved(FriendInfo friend) => FriendRemoved?.Invoke(this, new FriendEventArgs(friend));

        public EventHandler<FriendEventArgs> FriendUpdated; // TODO FriendUpdatedEventArgs ?
        protected virtual void OnFriendUpdated(FriendInfo friend) => FriendUpdated?.Invoke(this, new FriendEventArgs(friend));

        public FriendManager()
        {
            ResetFriends();
        }

        protected override void OnInitialize()
        {

        }

        private void ResetFriends()
        {
            lock (listLock)
            {
                Friends = new List<FriendInfo>().AsReadOnly();
            }
        }

        private void AddFriend(FriendInfo friend)
        {
            lock (listLock)
            {
                var list = new List<FriendInfo>(Friends);
                list.Add(friend);
                Friends = list.AsReadOnly();
            }
        }

        private void AddFriends(IEnumerable<FriendInfo> friends)
        {
            lock (listLock)
            {
                var list = new List<FriendInfo>(Friends);
                list.AddRange(friends);
                Friends = list.AsReadOnly();
            }
        }

        private void RemoveFriend(FriendInfo friend)
        {
            lock (listLock)
            {
                var list = new List<FriendInfo>(Friends);
                list.Remove(friend);
                Friends = list.AsReadOnly();
            }
        }

        [Group(Feature.Autoload), Receive("LatencyResponse"), RequiredOut("RequestInitFriends")]
        private async void HandleLatencyResponse(Packet packet)
        {
            if (!IsInitialized && !isForceLoading)
            {
                isForceLoading = true;
                await SendAsync(Out.RequestInitFriends);
            }
        }

        [InterceptIn("Friends")]
        private void HandleFriends(InterceptEventArgs e)
        {
            if (!isLoadingFriends) return;

            try
            {
                var packet = e.Packet;
                int total = packet.ReadInteger();
                int current = packet.ReadInteger();

                if (current != currentPacket) return;
                if (totalPackets == -1) totalPackets = total;
                else if (total != totalPackets) return;
                if (currentPacket == 0) loadList.Clear();
                currentPacket++;

                if (isForceLoading) e.Block();

                int n = packet.ReadInteger();
                for (int i = 0; i < n; i++)
                    loadList.Add(FriendInfo.Parse(packet));

                if (currentPacket == totalPackets)
                {
                    IsInitialized = true;
                    isLoadingFriends = false;
                    ResetFriends();
                    AddFriends(loadList);
                    loadList.Clear();
                    OnLoaded();
                }
            }
            catch { /* TODO handle this */ throw; }
        }


    }
}
