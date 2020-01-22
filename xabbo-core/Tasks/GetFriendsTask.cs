using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    internal class GetFriendsTask : InterceptorTask<List<FriendInfo>>
    {
        private int totalExpected = -1, currentIndex = 0;
        private readonly List<FriendInfo> friends = new List<FriendInfo>();

        public GetFriendsTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestInitFriends);

        [InterceptIn("Friends")]
        private void OnFriends(InterceptEventArgs e)
        {
            try
            {
                int total = e.Packet.ReadInteger();
                int current = e.Packet.ReadInteger();

                if (current != currentIndex) return;
                if (totalExpected == -1) totalExpected = total;
                else if (totalExpected != total) return;
                currentIndex++;

                e.Block();

                int n = e.Packet.ReadInteger();
                for (int i = 0; i < n; i++)
                    friends.Add(FriendInfo.Parse(e.Packet));

                if (currentIndex == total)
                    SetResult(friends);
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
