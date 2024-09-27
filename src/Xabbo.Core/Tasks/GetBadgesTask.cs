using System;
using System.Collections.Generic;

using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Core.Messages.Outgoing;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Tasks;

[Intercept]
public sealed partial class GetBadgesTask(IInterceptor interceptor)
    : InterceptorTask<List<Badge>>(interceptor)
{
    private readonly List<Badge> _badges = [];
    private int _totalExpected = -1, _currentIndex = 0;

    protected override ClientType SupportedClients => ~ClientType.Shockwave;

    protected override void OnExecute() => Interceptor.Send(new GetBadgesMsg());

    [Intercept]
    private void HandleBadges(Intercept<BadgeFragmentMsg> e)
    {
        try
        {
            if (e.Msg.FragmentIndex != _currentIndex) return;
            if (_totalExpected == -1) _totalExpected = e.Msg.TotalFragments;
            else if (e.Msg.TotalFragments != _totalExpected) return;
            _currentIndex++;

            e.Block();

            _badges.AddRange(e.Msg.Badges);
            if (_currentIndex == _totalExpected)
                SetResult(_badges);
        }
        catch (Exception ex) { SetException(ex); }
    }
}
