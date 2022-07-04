using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    public class GetFriendsTask : InterceptorTask<List<Friend>>
    {
        private int _totalExpected = -1, _currentIndex = 0;
        private readonly List<Friend> _friends = new List<Friend>();

        public GetFriendsTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.MessengerInit);

        [InterceptIn(nameof(Incoming.MessengerInit))]
        protected void OnInitFriends(InterceptArgs e) => e.Block();

        [InterceptIn(nameof(Incoming.FriendListFragment))]
        protected void OnFriends(InterceptArgs e)
        {
            try
            {
                int total = e.Packet.ReadInt();
                int current = e.Packet.ReadInt();

                if (current != _currentIndex) return;
                if (_totalExpected == -1) _totalExpected = total;
                else if (_totalExpected != total) return;
                _currentIndex++;

                e.Block();

                int n = e.Packet.ReadLegacyShort();
                for (int i = 0; i < n; i++)
                    _friends.Add(Friend.Parse(e.Packet));

                if (_currentIndex == total)
                    SetResult(_friends);
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
