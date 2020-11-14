using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xabbo.Core.Messages;
using Xabbo.Core.Tasks;

namespace Xabbo.Core.Components
{
    internal class InventoryManager : XabboComponent
    {
        class PassiveReceiveInventoryTask : GetInventoryTask
        {
            public PassiveReceiveInventoryTask(IInterceptor interceptor)
                : base(interceptor, false)
            { }

            // Don't request inventory items
            protected override Task OnExecuteAsync() => Task.CompletedTask;
        }

        public bool IsInitialized { get; private set; }
        public bool IsLoaded { get; private set; }
        public bool NeedsRefresh { get; private set; }

        private Inventory _inventory;
        public IInventory Inventory => _inventory;

        private Task<IInventory> _currentInventoryTask;

        protected override void OnInitialize()
        {
            Task.Run(() => MonitorInventoryAsync(Manager.DisposeToken));
        }

        private async Task MonitorInventoryAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _currentInventoryTask = new PassiveReceiveInventoryTask(Interceptor).ExecuteAsync(-1, cancellationToken)
                    .ContinueWith(t =>
                    {
                        _inventory = (Inventory)t.Result;
                        IsLoaded = true;
                        NeedsRefresh = false;
                        return t.Result;
                    });
                await _currentInventoryTask;
            }
        }

        [InterceptIn(nameof(Incoming.InventoryRefresh))]
        private void HandleInventoryRefresh(InterceptEventArgs e) => NeedsRefresh = true;

        [InterceptIn(nameof(Incoming.InventoryItemUpdate))]
        private void HandleInventoryItemUpdate(InterceptEventArgs e)
        {
            var newItem = InventoryItem.Parse(e.Packet);
            var existingItem = _inventory.FirstOrDefault<InventoryItem>(
                x => x.ItemId == newItem.Id && x.Type == newItem.Type
            );

            if (existingItem != null)
                _inventory.Remove(existingItem);

            _inventory.Add(newItem);
        }

        [InterceptIn(nameof(Incoming.RemoveHabboItem))]
        private void HandleRemoveHabboItem(InterceptEventArgs e)
        {
            // TODO
        }

        public async Task<IInventory> LoadInventoryAsync(int timeout, CancellationToken cancellationToken)
        {
            if (!IsLoaded || NeedsRefresh)
            {
                await SendAsync(Out.RequestInventoryItems);

                var cts = CancellationTokenSource.CreateLinkedTokenSource(Manager.DisposeToken, cancellationToken);
                if (timeout > 0) cts.CancelAfter(timeout);

                try
                {
                    var timeoutTask = Task.Delay(-1, cts.Token);
                    await Task.WhenAny(_currentInventoryTask, timeoutTask);
                }
                finally { cts.Dispose(); }
            }

            return Inventory;
        }
    }
}
