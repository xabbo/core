using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut("RequestInitFriends")]
    public class GetFriendsTask : InterceptorTask<List<Friend>>
    {
        private int totalExpected = -1, currentIndex = 0;
        private readonly List<Friend> friends = new List<Friend>();

        public GetFriendsTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestInitFriends);

        [InterceptIn(nameof(Incoming.InitFriends))]
        protected void OnInitFriends(InterceptEventArgs e) => e.Block();

        [InterceptIn(nameof(Incoming.Friends))]
        protected void OnFriends(InterceptEventArgs e)
        {
            try
            {
                int total = e.Packet.ReadInt();
                int current = e.Packet.ReadInt();

                if (current != currentIndex) return;
                if (totalExpected == -1) totalExpected = total;
                else if (totalExpected != total) return;
                currentIndex++;

                e.Block();

                int n = e.Packet.ReadInt();
                for (int i = 0; i < n; i++)
                    friends.Add(Friend.Parse(e.Packet));

                if (currentIndex == total)
                    SetResult(friends);
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
