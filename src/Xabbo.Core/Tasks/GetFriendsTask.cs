using System;
using System.Collections.Generic;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Tasks;

[Intercept]
public sealed partial class GetFriendsTask(IInterceptor interceptor) : InterceptorTask<List<Friend>>(interceptor)
{
    private int _totalExpected = -1, _currentIndex = 0;
    private readonly List<Friend> _friends = [];

    protected override void OnExecute() => Interceptor.Send(Out.MessengerInit);

    [Intercept]
    void OnInitFriends(Intercept<MessengerInitMsg> e)
    {
        try
        {
            e.Block();

            if (Session.Is(ClientType.Origins))
            {
                SetResult(e.Msg.Friends);
            }
        }
        catch (Exception ex) { SetException(ex); }
    }

    [Intercept(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.FriendListFragment))]
    void OnFriends(Intercept e)
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
