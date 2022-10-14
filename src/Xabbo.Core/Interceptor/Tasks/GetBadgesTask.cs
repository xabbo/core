using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetBadgesTask : InterceptorTask<List<Badge>>
{
    private int _totalExpected = -1, _currentIndex = 0;
    private readonly List<Badge> _badges = new List<Badge>();

    public GetBadgesTask(IInterceptor interceptor)
        : base(interceptor)
    { }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.GetAvailableBadges);

    [InterceptIn(nameof(Incoming.AvailableBadges))]
    protected void OnInventoryBadges(InterceptArgs e)
    {
        try
        {
            var packet = e.Packet;
            int total = packet.ReadInt();
            int index = packet.ReadInt();

            if (index != _currentIndex) return;
            if (_totalExpected == -1) _totalExpected = total;
            else if (total != _totalExpected) return;
            _currentIndex++;

            e.Block();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                _badges.Add(Badge.Parse(packet));

            if (_currentIndex == _totalExpected)
                SetResult(_badges);
        }
        catch (Exception ex) { SetException(ex); }
    }
}
