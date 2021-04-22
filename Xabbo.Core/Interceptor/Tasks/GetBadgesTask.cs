using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    // @Update [RequiredOut(nameof(Outgoing.RequestInventoryBadges))]
    public class GetBadgesTask : InterceptorTask<List<Badge>>
    {
        private int totalExpected = -1, currentIndex = 0;
        private readonly List<Badge> badges = new List<Badge>();

        public GetBadgesTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => throw new NotImplementedException(); // @Update  SendAsync(Out.RequestInventoryBadges);

        // @Update [InterceptIn(nameof(Incoming.InventoryBadges))]
        protected void OnInventoryBadges(InterceptArgs e)
        {
            try
            {
                var packet = e.Packet;
                int total = packet.ReadInt();
                int index = packet.ReadInt();

                if (index != currentIndex) return;
                if (totalExpected == -1) totalExpected = total;
                else if (total != totalExpected) return;
                currentIndex++;

                e.Block();

                int n = packet.ReadInt();
                for (int i = 0; i < n; i++)
                    badges.Add(Badge.Parse(packet));

                if (currentIndex == totalExpected)
                    SetResult(badges);
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
