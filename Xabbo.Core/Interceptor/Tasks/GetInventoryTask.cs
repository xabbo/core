using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

using Xabbo.Core.Game;

namespace Xabbo.Core.Tasks
{
    public class GetInventoryTask : InterceptorTask<IInventory>
    {
        private int _total = -1, _index = 0;
        private readonly Inventory inventory = new Inventory();

        private readonly bool _blockPackets;

        public GetInventoryTask(IInterceptor interceptor, bool blockPackets = true)
            : base(interceptor)
        {
            _blockPackets = blockPackets;
        }

        public GetInventoryTask(IInterceptor interceptor) : this(interceptor, true) { }

        protected override Task OnExecuteAsync() => SendAsync(Out.GetInventory);

        [InterceptIn(nameof(Incoming.InventoryPush))]
        protected void OnInventoryItems(InterceptArgs e)
        {
            try
            {
                InventoryFragment fragment = InventoryFragment.Parse(e.Packet);

                if (fragment.Index != _index)
                {
                    throw new Exception(
                        $"Fragment index mismatch."
                        + $" Expected: {_index}; received: {fragment.Index}."
                    );
                }

                if (_total == -1)
                {
                    _total = fragment.Total;
                }
                else if (fragment.Total != _total)
                {
                    throw new Exception(
                        "Fragment count mismatch."
                        + $" Expected: {_total}; received: {fragment.Total}."
                    );
                }

                _index++;

                if (_blockPackets)
                    e.Block();

                foreach (var item in fragment)
                    inventory.TryAdd(item);

                if (_index == _total)
                    SetResult(inventory);
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
