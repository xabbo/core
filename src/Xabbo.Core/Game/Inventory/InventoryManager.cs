using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Extension;
using Xabbo.Messages.Flash;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

/// <summary>
/// Manages the user's inventory.
/// </summary>
[Intercept(~ClientType.Shockwave)]
public sealed partial class InventoryManager(IExtension extension, ILoggerFactory? loggerFactory = null) : GameStateManager(extension)
{
    private readonly ILogger? Log = loggerFactory?.CreateLogger<InventoryManager>();

    private readonly List<InventoryFragment> _fragments = [];
    private bool _forceLoadingInventory;
    private int _currentPacketIndex;
    private int _totalPackets;

    private TaskCompletionSource<IInventory> _loadTcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private Inventory? _inventory;
    public IInventory? Inventory => _inventory;

    public event Action? Invalidated;
    private void OnInvalidated() => Invalidated?.Invoke();

    public event Action? Loaded;
    private void OnLoaded() => Loaded?.Invoke();

    public event Action<InventoryItemEventArgs>? ItemAdded;
    private void OnItemAdded(InventoryItem item) => ItemAdded?.Invoke(new InventoryItemEventArgs(item));

    public event Action<InventoryItemEventArgs>? ItemUpdated;
    private void OnItemUpdated(InventoryItem item) => ItemUpdated?.Invoke(new InventoryItemEventArgs(item));

    public event Action<InventoryItemEventArgs>? ItemRemoved;
    private void OnItemRemoved(InventoryItem item) => ItemRemoved?.Invoke(new InventoryItemEventArgs(item));

    protected override void OnDisconnected()
    {
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
    public async Task<IInventory> GetInventoryAsync(int timeout = XabboConst.DefaultTimeout,
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
                    Interceptor.Send(Out.RequestFurniInventory);
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

    [InterceptIn(nameof(In.FurniListInvalidate))]
    private void HandleFurniListInvalidate(Intercept e)
    {
        Log.LogTrace("Inventory invalidated.");

        if (_inventory is not null)
        {
            _inventory.IsInvalidated = true;
        }

        OnInvalidated();
    }

    [InterceptIn(nameof(In.FurniList))]
    private void HandleFurniList(Intercept e)
    {
        if (_forceLoadingInventory) e.Block();

        InventoryFragment fragment = e.Packet.Read<InventoryFragment>();

        if (fragment.Index == 0)
        {
            Log.LogTrace("Resetting inventory load state.");

            _currentPacketIndex = 0;
            _totalPackets = fragment.Total;
            _fragments.Clear();
        }
        else if (_currentPacketIndex != fragment.Index ||
            _totalPackets != fragment.Total)
        {
            Log.LogWarning(
                "Inventory load state mismatch!"
                + " Expected {expectedIndex}/{expectedTotal};"
                + " received {actualIndex}/{actualTotal} (index/total).",
                _currentPacketIndex, _totalPackets,
                fragment.Index, fragment.Total
            );
            return;
        }

        Log.LogTrace("Received inventory fragment {n} of {total}.", fragment.Index + 1, fragment.Total);

        _currentPacketIndex++;
        _fragments.Add(fragment);

        if (fragment.Index == (fragment.Total - 1))
        {
            Log.LogTrace("All inventory fragments received.");

            _inventory ??= new Inventory();
            _inventory.Clear();
            _inventory.IsInvalidated = false;

            foreach (InventoryItem item in _fragments.SelectMany(fragment => (ICollection<InventoryItem>)fragment))
            {
                if (!_inventory.TryAdd(item))
                {
                    Log.LogWarning("Failed to add inventory item {itemId}!", item.ItemId);
                }
            }

            _forceLoadingInventory = false;

            SetLoadTaskResult(_inventory);
            OnLoaded();
        }
    }

    [InterceptIn(nameof(In.FurniListAddOrUpdate))]
    private void HandleInventoryAddOrUpdateFurni(Intercept e)
    {
        if (_inventory is null) return;

        InventoryItem item = e.Packet.Read<InventoryItem>();
        _inventory.AddOrUpdate(item, out bool added);

        if (added)
        {
            Log.LogTrace("Added inventory item {id}.", item.Id);
            OnItemAdded(item);
        }
        else
        {
            Log.LogTrace("Updated inventory item {id}.", item.Id);
            OnItemUpdated(item);
        }
    }

    [InterceptIn(nameof(In.FurniListRemove))]
    private void HandleFurniListRemove(Intercept e)
    {
        if (_inventory is null) return;

        long itemId = e.Packet.Read<Id>();
        if (_inventory.TryRemove(itemId, out InventoryItem? item))
        {
            Log.LogTrace("Inventory item {id} removed.", itemId);
            OnItemRemoved(item);
        }
        else
        {
            Log.LogWarning("Failed to find inventory item {id} to remove!", itemId);
        }
    }
}
