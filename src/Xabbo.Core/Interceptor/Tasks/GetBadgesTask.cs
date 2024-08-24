using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

[Intercepts]
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
            var packet = e.Packet;
            int total = packet.Read<int>();
            int index = packet.Read<int>();

            if (index != _currentIndex) return;
            if (_totalExpected == -1) _totalExpected = total;
            else if (total != _totalExpected) return;
            _currentIndex++;

            e.Block();

            int n = packet.Read<int>();
            for (int i = 0; i < n; i++)
                _badges.Add(packet.Parse<Badge>());

            if (_currentIndex == _totalExpected)
                SetResult(_badges);
        }
        catch (Exception ex) { SetException(ex); }
    }
}
