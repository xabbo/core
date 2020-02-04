using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    [RequiredOut("RequestInventoryBadges")]
    public class GetBadgesTask : InterceptorTask<List<Badge>>
    {
        private int totalExpected = -1, currentIndex = 0;
        private readonly List<Badge> badges = new List<Badge>();

        public GetBadgesTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestInventoryBadges);

        [InterceptIn("InventoryBadges")]
        private void OnInventoryBadges(InterceptEventArgs e)
        {
            try
            {
                var packet = e.Packet;
                int total = packet.ReadInteger();
                int index = packet.ReadInteger();

                if (index != currentIndex) return;
                if (totalExpected == -1) totalExpected = total;
                else if (total != totalExpected) return;
                currentIndex++;

                e.Block();

                int n = packet.ReadInteger();
                for (int i = 0; i < n; i++)
                    badges.Add(Badge.Parse(packet));

                if (currentIndex == totalExpected)
                    SetResult(badges);
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
