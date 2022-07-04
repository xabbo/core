using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Messages;
using Xabbo.Interceptor;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game
{
    /// <summary>
    /// Manages the user's inventory.
    /// </summary>
    public class InventoryManager : GameStateManager
    {
        private readonly ILogger _logger;

        private readonly List<InventoryFragment> _fragments = new();
        private bool _forceLoadingInventory;
        private int _currentPacketIndex;
        private int _totalPackets;

        private TaskCompletionSource<IInventory> _loadTcs
            = new TaskCompletionSource<IInventory>(TaskCreationOptions.RunContinuationsAsynchronously);

        private Inventory? _inventory;
        public IInventory? Inventory => _inventory;

        public event EventHandler? Invalidated;
        protected virtual void OnInvalidated()
            => Invalidated?.Invoke(this, EventArgs.Empty);

        public event EventHandler? Loaded;
        protected virtual void OnLoaded()
            => Loaded?.Invoke(this, EventArgs.Empty);

        public event EventHandler<InventoryItemEventArgs>? ItemAdded;
        protected virtual void OnItemAdded(InventoryItem item)
            => ItemAdded?.Invoke(this, new InventoryItemEventArgs(item));

        public event EventHandler<InventoryItemEventArgs>? ItemUpdated;
        protected virtual void OnItemUpdated(InventoryItem item)
            => ItemUpdated?.Invoke(this, new InventoryItemEventArgs(item));

        public event EventHandler<InventoryItemEventArgs>? ItemRemoved;
        protected virtual void OnItemRemoved(InventoryItem item)
            => ItemRemoved?.Invoke(this, new InventoryItemEventArgs(item));

        public InventoryManager(ILogger<InventoryManager> logger, IInterceptor interceptor)
            : base(interceptor)
        {
            _logger = logger;
        }

        public InventoryManager(IInterceptor interceptor)
            : base(interceptor)
        {
            _logger = NullLogger.Instance;
        }

        protected override void OnDisconnected(object? sender, EventArgs e)
        {
            base.OnDisconnected(sender, e);

            _inventory = null;
            _forceLoadingInventory = false;
            _currentPacketIndex = 0;
            _totalPackets = 0;
        }

        /// <summary>
        /// Returns the inventory immediately if it is available
        /// and has not been invalidated, otherwise attempts to retrieve it from the server.
        /// Note that the user must be in a room to retrieve the inventory from the server.
        /// If the user is not in a room and a request to load the inventory is made, this method will time out.
        /// </summary>
        public async Task<IInventory> GetInventoryAsync(int timeout = XabboConst.DEFAULT_TIMEOUT,
            CancellationToken cancellationToken = default)
        {
            if (_inventory?.IsInvalidated == false)
            {
                return _inventory;
            }
            else
            {
                Task<IInventory> loadTask = _loadTcs.Task;

                CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                if (timeout > 0) cts.CancelAfter(timeout);

                try
                {
                    if (!_forceLoadingInventory)
                    {
                        await Interceptor.SendAsync(Out.GetInventory);
                        _forceLoadingInventory = true;
                    }
                    
                    await await Task.WhenAny(loadTask, Task.Delay(Timeout.Infinite, cts.Token));
                    return await loadTask;
                }
                finally { cts.Dispose(); }
            }
        }

        private void SetLoadTaskResult(IInventory inventory)
        {
            _loadTcs.TrySetResult(inventory);
            _loadTcs = new TaskCompletionSource<IInventory>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        [InterceptIn(nameof(Incoming.InventoryInvalidate))]
        protected void HandleInventoryInvalidate(InterceptArgs e)
        {
            _logger.LogTrace("Inventory invalidated.");

            if (_inventory is not null)
            {
                _inventory.IsInvalidated = true;
            }

            OnInvalidated();
        }

        [InterceptIn(nameof(Incoming.InventoryPush))]
        protected void HandleInventoryPush(InterceptArgs e)
        {
            if (_forceLoadingInventory) e.Block();

            InventoryFragment fragment = InventoryFragment.Parse(e.Packet);

            if (fragment.Index == 0)
            {
                _logger.LogTrace("Resetting inventory load state.");

                _currentPacketIndex = 0;
                _totalPackets = fragment.Total;
                _fragments.Clear();
            }
            else if (_currentPacketIndex != fragment.Index ||
                _totalPackets != fragment.Total)
            {
                _logger.LogWarning(
                    "Inventory load state mismatch!"
                    + " Expected {expectedIndex}/{expectedTotal};"
                    + " received {actualIndex}/{actualTotal} (index/total).",
                    _currentPacketIndex, _totalPackets,
                    fragment.Index, fragment.Total
                );
                return;
            }

            _logger.LogTrace("Received inventory fragment {n} of {total}.", fragment.Index + 1, fragment.Total);

            _currentPacketIndex++;
            _fragments.Add(fragment);

            if (fragment.Index == (fragment.Total - 1))
            {
                _logger.LogTrace("All inventory fragments received.");

                _inventory ??= new Inventory();
                _inventory.Clear();
                _inventory.IsInvalidated = false;

                foreach (InventoryItem item in _fragments.SelectMany(fragment => (ICollection<InventoryItem>)fragment))
                {
                    if (!_inventory.TryAdd(item))
                    {
                        _logger.LogWarning("Failed to add inventory item {itemId}!", item.ItemId);
                    }
                }

                _forceLoadingInventory = false;

                SetLoadTaskResult(_inventory);
                OnLoaded();
            }
        }

        [InterceptIn(nameof(Incoming.InventoryAddOrUpdateFurni))]
        protected virtual void HandleInventoryAddOrUpdateFurni(InterceptArgs e)
        {
            if (_inventory is null) return;

            InventoryItem item = InventoryItem.Parse(e.Packet);
            _inventory.AddOrUpdate(item, out bool added);

            if (added)
            {
                _logger.LogTrace("Added inventory item {id}.", item.Id);
                OnItemAdded(item);
            }
            else
            {
                _logger.LogTrace("Updated inventory item {id}.", item.Id);
                OnItemUpdated(item);
            }
        }

        [InterceptIn(nameof(Incoming.InventoryRemoveFurni))]
        protected virtual void HandleInventoryRemoveFurni(InterceptArgs e)
        {
            if (_inventory is null) return;

            long itemId = e.Packet.ReadLegacyLong();
            if (_inventory.TryRemove(itemId, out InventoryItem? item))
            {
                _logger.LogTrace("Inventory item {id} removed.", itemId);
                OnItemRemoved(item);
            }
            else
            {
                _logger.LogWarning("Failed to find inventory item {id} to remove!", itemId);
            }
        }
    }
}
