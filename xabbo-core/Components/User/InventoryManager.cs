using System;
using System.Collections;
using System.Collections.Concurrent;
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
        class InventoryInternal : IInventory
        {
            private readonly ConcurrentDictionary<int, IInventoryItem> _items;

            public int Count => _items.Count;

            public IEnumerable<IInventoryItem> FloorItems => this.Where(x => x.Type == ItemType.Floor);
            public IEnumerable<IInventoryItem> WallItems => this.Where(x => x.Type == ItemType.Wall);

            public InventoryInternal(IInventory inventory)
            {
                _items = new ConcurrentDictionary<int, IInventoryItem>(
                    inventory.Select(x => new KeyValuePair<int, IInventoryItem>(
                        x.ItemId, x
                    ))
                );
            }

            internal void UpdateItem(IInventoryItem item)
            {
                _items.AddOrUpdate(item.ItemId, item, (key, existing) => item);
            }

            public IEnumerator<IInventoryItem> GetEnumerator() => _items.Select(x => x.Value).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        class ReceiveInventoryTask : GetInventoryTask
        {
            public ReceiveInventoryTask(IInterceptor interceptor)
                : base(interceptor, false)
            { }

            // Don't request inventory items
            protected override Task OnExecuteAsync() => Task.CompletedTask;
        }

        public bool IsInitialized { get; private set; }
        public bool IsLoaded { get; private set; }
        public bool NeedsRefresh { get; private set; }

        private InventoryInternal _inventory;
        public IInventory Inventory => _inventory;

        private Task<IInventory> _currentInventoryTask;



        private Task<IInventory> _loadTask;



        protected override void OnInitialize()
        {
            // Task.Run(() => MonitorInventoryAsync(Manager.DisposeToken));
        }

        private async Task<IInventory> ReceiveInventoryAsync(CancellationToken ct)
        {
            return new InventoryInternal(
                await new ReceiveInventoryTask(Interceptor).ExecuteAsync(-1, ct)
            );
        }

       /* private async Task MonitorInventoryAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                _currentInventoryTask = new ReceiveInventoryTask(Interceptor).ExecuteAsync(-1, ct)
                    .ContinueWith(t =>
                    {
                        _inventory = new InventoryInternal(t.Result);
                        IsLoaded = true;
                        NeedsRefresh = false;
                        return (IInventory)_inventory;
                    });

                await _currentInventoryTask;
            }
        }*/

        [InterceptIn(nameof(Incoming.InventoryRefresh))]
        private void HandleInventoryRefresh(InterceptEventArgs e)
        {
            NeedsRefresh = true;
            
            if (_loadTask.IsCompleted)
                _loadTask = ReceiveInventoryAsync(Manager.DisposeToken);
        }

        [InterceptIn(nameof(Incoming.InventoryItemUpdate))]
        private void HandleInventoryItemUpdate(InterceptEventArgs e)
        {
            _inventory.UpdateItem(InventoryItem.Parse(e.Packet));
        }

        [InterceptIn(nameof(Incoming.AddHabboItem))]
        private void HandleAddHabboItem(InterceptEventArgs e)
        {
            
            /*
             * Dictionary<int, List<int>>
             * 
            int {
                int local_2
                int local_3 {
                    int
                }
            }
            */
        }

        [InterceptIn(nameof(Incoming.RemoveHabboItem))]
        private void HandleRemoveHabboItem(InterceptEventArgs e)
        {
            // int itemId
        }

        public async Task<IInventory> GetInventoryAsync(int timeout, CancellationToken ct)
        {
            if (!IsLoaded || NeedsRefresh)
            {
                CancellationTokenSource cts = null;

                try
                {
                    cts = CancellationTokenSource.CreateLinkedTokenSource(Manager.DisposeToken, ct);
                    if (timeout > 0) cts.CancelAfter(timeout);

                    var inventoryTask = _currentInventoryTask;
                    await SendAsync(Out.RequestInventoryItems);

                    var timeoutTask = Task.Delay(-1, cts.Token);
                    await Task.WhenAny(inventoryTask, timeoutTask);
                }
                finally { cts?.Dispose(); }
            }

            return Inventory;
        }
    }
}
