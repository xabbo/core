using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut("RequestInventoryItems")]
    public class GetInventoryTask : InterceptorTask<Inventory>
    {
        private int totalExpected = -1, currentIndex = 0;
        private readonly Inventory inventory = new Inventory();

        public GetInventoryTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestInventoryItems);

        [InterceptIn("InventoryItems")]
        private void OnInventoryItems(InterceptEventArgs e)
        {
            try
            {
                var packet = e.Packet;
                int total = packet.ReadInt();
                int current = packet.ReadInt();

                if (current != currentIndex) return;
                if (totalExpected == -1) totalExpected = total;
                else if (total != totalExpected) return;
                currentIndex++;

                e.Block();

                inventory.AddRange(Inventory.ParseItems(packet));
                if (currentIndex == totalExpected)
                    SetResult(inventory);
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
