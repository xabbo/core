using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetFriendsTask : InterceptorTask<List<Friend>>
{
    private int _totalExpected = -1, _currentIndex = 0;
    private readonly List<Friend> _friends = new List<Friend>();

    public GetFriendsTask(IInterceptor interceptor)
        : base(interceptor)
    { }

    protected override void OnExecute() => Interceptor.Send(Out.MessengerInit);

    [InterceptIn(nameof(In.MessengerInit))]
    protected void OnInitFriends(Intercept e) => e.Block();

    [InterceptIn(nameof(In.FriendListFragment))]
    protected void OnFriends(Intercept e)
    {
        try
        {
            int total = e.Packet.Read<int>();
            int current = e.Packet.Read<int>();

            if (current != _currentIndex) return;
            if (_totalExpected == -1) _totalExpected = total;
            else if (_totalExpected != total) return;
            _currentIndex++;

            e.Block();

            int n = e.Packet.Read<Length>();
            for (int i = 0; i < n; i++)
                _friends.Add(e.Packet.Read<Friend>());

            if (_currentIndex == total)
                SetResult(_friends);
        }
        catch (Exception ex) { SetException(ex); }
    }
}
