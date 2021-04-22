using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    // @Update [RequiredOut("RequestInitFriends")]
    public class GetFriendsTask : InterceptorTask<List<Friend>>
    {
        private int totalExpected = -1, currentIndex = 0;
        private readonly List<Friend> friends = new List<Friend>();

        public GetFriendsTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => throw new NotImplementedException(); // @Update SendAsync(Out.RequestInitFriends);

        // @Update [InterceptIn(nameof(Incoming.InitFriends))]
        protected void OnInitFriends(InterceptArgs e) => e.Block();

        // @Update [InterceptIn(nameof(Incoming.Friends))]
        protected void OnFriends(InterceptArgs e)
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
