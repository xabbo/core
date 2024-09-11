using System;
using System.Collections.Generic;

using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Tasks;

[Intercept]
public sealed partial class GetBadgesTask(IInterceptor interceptor)
    : InterceptorTask<List<Badge>>(interceptor)
{
    private int _totalExpected = -1, _currentIndex = 0;
    private readonly List<Badge> _badges = [];

    protected override void OnExecute() => Interceptor.Send(Out.GetBadges);

    [InterceptIn(nameof(In.Badges))]
    private void HandleBadges(Intercept e)
    {
        try
        {
            var (total, index) = e.Packet.Read<int, int>();

            if (index != _currentIndex) return;
            if (_totalExpected == -1) _totalExpected = total;
            else if (total != _totalExpected) return;
            _currentIndex++;

            e.Block();

            _badges.AddRange(e.Packet.Read<Badge[]>());
            if (_currentIndex == _totalExpected)
                SetResult(_badges);
        }
        catch (Exception ex) { SetException(ex); }
    }
}
