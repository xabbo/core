﻿using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut(nameof(Outgoing.RequestInventoryItems))]
    public class GetInventoryTask : InterceptorTask<IInventory>
    {
        private int totalExpected = -1, currentIndex = 0;
        private readonly Inventory inventory = new Inventory();

        private readonly bool _block;

        public GetInventoryTask(IInterceptor interceptor, bool blockPackets)
            : base(interceptor)
        {
            _block = blockPackets;
        }

        public GetInventoryTask(IInterceptor interceptor) : this(interceptor, true) { }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestInventoryItems);

        [InterceptIn(nameof(Incoming.InventoryItems))]
        protected void OnInventoryItems(InterceptEventArgs e)
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

                if (_block)
                    e.Block();

                foreach (var item in Inventory.ParseItems(packet))
                    inventory.Add(item);
                if (currentIndex == totalExpected)
                    SetResult(inventory);
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}