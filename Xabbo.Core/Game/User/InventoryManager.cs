using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xabbo.Core.Events;
using Xabbo.Messages;
using Xabbo.Core.Tasks;

namespace Xabbo.Core.Game
{
#if FALSE
    public class InventoryManager : GameStateManager
    {
        class InventoryInternal : IInventory
        {
            private readonly ConcurrentDictionary<long, IInventoryItem> _items;

            public int Count => _items.Count;

            public IEnumerable<IInventoryItem> FloorItems => this.Where(x => x.Type == ItemType.Floor);
            public IEnumerable<IInventoryItem> WallItems => this.Where(x => x.Type == ItemType.Wall);

            public InventoryInternal(IInventory inventory)
            {
                _items = new ConcurrentDictionary<long, IInventoryItem>(
                    inventory.Select(x => new KeyValuePair<long, IInventoryItem>(
                        x.ItemId, x
                    ))
                );
            }

            /// <summary>
            /// Adds or updates the inventory item and returns whether the item was added.
            /// </summary>
            internal bool AddOrUpdate(IInventoryItem item)
            {
                _items.AddOrUpdate(item.ItemId, item, (key, existing) => item, out bool added);
                return added;
            }

            internal bool TryRemove(int itemId, out IInventoryItem item) => _items.TryRemove(itemId, out item);

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
        public bool IsRefreshNeeded { get; private set; }

        private InventoryInternal _inventory;
        public IInventory Inventory => _inventory;

        private Task<IInventory> _loadTask;

        public event EventHandler RefreshNeeded;
        protected virtual void OnRefreshNeeded() => RefreshNeeded?.Invoke(this, EventArgs.Empty);

        public event EventHandler InventoryLoaded;
        protected virtual void OnInventoryLoaded() => InventoryLoaded?.Invoke(this, EventArgs.Empty);

        public event EventHandler<InventoryItemEventArgs> ItemAdded;
        protected virtual void OnItemAdded(IInventoryItem item)
            => ItemAdded?.Invoke(this, new InventoryItemEventArgs(item));

        public event EventHandler<InventoryItemEventArgs> ItemUpdated;
        protected virtual void OnItemUpdated(IInventoryItem item)
            => ItemUpdated?.Invoke(this, new InventoryItemEventArgs(item));

        public event EventHandler<InventoryItemEventArgs> ItemRemoved;
        protected virtual void OnItemRemoved(IInventoryItem item)
            => ItemRemoved?.Invoke(this, new InventoryItemEventArgs(item));

        protected override void OnInitialize()
        {
            _loadTask = ReceiveInventoryAsync(Manager.DisposeToken);
        }

        private async Task<IInventory> ReceiveInventoryAsync(CancellationToken ct)
        {
            var inventory = await new ReceiveInventoryTask(Interceptor).ExecuteAsync(-1, ct);
            
            _inventory = new InventoryInternal(inventory);
            IsLoaded = true;
            IsRefreshNeeded = false;

            OnInventoryLoaded();

            return Inventory;
        }

        // @Update [InterceptIn(nameof(Incoming.InventoryRefresh))]
        private void HandleInventoryRefresh(InterceptArgs e)
        {
            IsRefreshNeeded = true;
            
            if (_loadTask.IsCompleted)
                _loadTask = ReceiveInventoryAsync(Manager.DisposeToken);
        }

        // @Update [InterceptIn(nameof(Incoming.InventoryItemUpdate))]
        private void HandleInventoryItemUpdate(InterceptArgs e)
        {
            if (_inventory != null)
            {
                var item = InventoryItem.Parse(e.Packet);
                if (_inventory.AddOrUpdate(item))
                {
                    OnItemAdded(item);
                }
                else
                {
                    OnItemUpdated(item);
                }
            }
        }

        // @Update [InterceptIn(nameof(Incoming.AddHabboItem))]
        private void HandleAddHabboItem(InterceptArgs e)
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

            // ???
        }

        // @Update [InterceptIn(nameof(Incoming.RemoveHabboItem))]
        private void HandleRemoveHabboItem(InterceptArgs e)
        {
            int itemId = e.Packet.ReadInt();

            if (_inventory.TryRemove(itemId, out IInventoryItem item))
            {
                OnItemRemoved(item);
            }
        }

        public async Task<IInventory> RefreshInventoryAsync(int timeout, CancellationToken ct)
        {
            if (_loadTask.IsCompleted)
                _loadTask = ReceiveInventoryAsync(Manager.DisposeToken);

            CancellationTokenSource cts = null;

            try
            {
                cts = CancellationTokenSource.CreateLinkedTokenSource(Manager.DisposeToken, ct);
                if (timeout > 0) cts.CancelAfter(timeout);

                var loadTask = _loadTask;
                await SendAsync(Out.GetInventory);

                var timeoutTask = Task.Delay(-1, cts.Token);
                await await Task.WhenAny(loadTask, timeoutTask);

                return loadTask.Result;
            }
            finally { cts?.Dispose(); }
        }

        public async Task<IInventory> GetInventoryAsync(int timeout, CancellationToken ct)
        {
            // if (!IsLoaded || IsRefreshNeeded)
            if (!_loadTask.IsCompleted)
            {
                return await RefreshInventoryAsync(timeout, ct);
            }

            return Inventory;
        }
    }
#endif
}
