using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Extension;
using Xabbo.Messages.Flash;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

/// <summary>
/// Manages the user's inventory.
/// </summary>
[Intercept]
public sealed partial class InventoryManager : GameStateManager
{
    private class InventoryItemIdComparer : IEqualityComparer<InventoryItem>
    {
        public static readonly InventoryItemIdComparer Default = new InventoryItemIdComparer();

        public int GetHashCode([DisallowNull] InventoryItem obj)
        {
            return obj is InventoryItem inventoryItem ? inventoryItem.ItemId.GetHashCode() : obj.GetHashCode();
        }

        public bool Equals(InventoryItem? x, InventoryItem? y)
        {
            return x?.ItemId == y?.ItemId;
        }
    }

    private readonly ILogger _logger;
    private readonly RoomManager _roomManager;
    private readonly TradeManager _tradeManager;

    private readonly SemaphoreSlim _loadSemaphore = new(1, 1);

    private readonly List<InventoryFragment> _fragments = [];
    private int _currentFragmentIndex;
    private int _totalFragments;

    private TaskCompletionSource<IEnumerable<InventoryItem>>? _loadInventoryItemsTcs;
    private Task<IInventory>? _loadInventoryTask;

    private Inventory? _inventory;

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => Set(ref _isLoading, value);
    }

    private int _currentProgress;
    public int CurrentProgress
    {
        get => _currentProgress;
        set => Set(ref _currentProgress, value);
    }

    private int _maxProgress = -1;
    public int MaxProgress
    {
        get => _maxProgress;
        set => Set(ref _maxProgress, value);
    }

    public InventoryManager(IExtension extension,
        RoomManager roomManager,
        TradeManager tradeManager,
        ILoggerFactory? loggerFactory = null
    )
        : base(extension)
    {
        _logger = (ILogger?)loggerFactory?.CreateLogger<InventoryManager>() ?? NullLogger.Instance;
        _roomManager = roomManager;
        _tradeManager = tradeManager;

        _roomManager.FloorItemRemoved += OnFloorItemRemoved;
        _roomManager.WallItemRemoved += OnWallItemRemoved;

        _tradeManager.Completed += OnTradeCompleted;
    }

    public IInventory? Inventory => _inventory;

    protected override void OnDisconnected()
    {
        _inventory = null;
        _currentFragmentIndex = 0;
        _totalFragments = 0;
        Cleared?.Invoke();
    }

    /// <summary>
    /// Returns the user's inventory immediately if it is available
    /// and has not been invalidated, otherwise attempts to retrieve it from the server.
    /// </summary>
    /// <remarks>
    /// The user must be in a room to retrieve the inventory from the server.
    /// If the user is not in a room and a request to load the inventory is made, this method will time out.
    /// </remarks>
    public Task<IInventory> LoadInventoryAsync(
        int timeout = XabboConst.DefaultTimeout,
        int scanInterval = XabboConst.DefaultOriginsInventoryScanInterval,
        bool forceReload = false,
        CancellationToken cancellationToken = default)
    {
        if (!forceReload && Inventory is { IsInvalidated: false } inventory)
            return Task.FromResult((IInventory)inventory);

        return LoadInventoryInternalAsync(timeout, scanInterval, forceReload, cancellationToken);
    }

    private async Task<IInventory> LoadInventoryInternalAsync(
        int timeout, int scanInterval, bool forceReload,
        CancellationToken cancellationToken)
    {
        TaskCompletionSource<IInventory> loadInventoryTcs;
        Task<IInventory> loadInventoryTask;
        Task<IEnumerable<InventoryItem>>? loadItemsTask = null;
        CancellationTokenSource? cts = null;

        _loadSemaphore.Wait(cancellationToken);
        try
        {
            if (_loadInventoryTask is not null)
                return await _loadInventoryTask;

            if (!forceReload && _inventory is { IsInvalidated: false } currentInventory)
                return currentInventory;

            _inventory = null;
            _fragments.Clear();
            CurrentProgress = 1;
            MaxProgress = -1;
            IsLoading = true;

            loadInventoryTcs = new TaskCompletionSource<IInventory>(TaskCreationOptions.RunContinuationsAsynchronously);
            _loadInventoryTask = loadInventoryTask = loadInventoryTcs.Task;

            cts = CancellationTokenSource.CreateLinkedTokenSource(Interceptor.DisconnectToken, cancellationToken);
            cts.Token.Register(() => loadInventoryTcs.TrySetCanceled());
            if (timeout > 0)
                cts.CancelAfter(timeout);

            loadItemsTask = Interceptor.Session.Is(ClientType.Origins)
                ? LoadInventoryOriginsAsync(scanInterval, cts.Token)
                : LoadInventoryModernAsync(cts.Token);
        }
        finally { _loadSemaphore.Release(); }

        Cleared?.Invoke();

        IEnumerable<InventoryItem>? items = null;
        Inventory? inventory = null;

        try
        {
            items = await loadItemsTask;
        }
        catch (Exception ex)
        {
            loadInventoryTcs.TrySetException(ex);
            throw;
        }
        finally
        {
            _loadSemaphore.Wait(CancellationToken.None);
            try
            {
                if (items is not null)
                {
                    _inventory = inventory = new Inventory();
                    foreach (InventoryItem item in items)
                    {
                        if (!_inventory.TryAdd(item))
                        {
                            _logger.LogWarning("Failed to add inventory item {itemId}!", item.ItemId);
                        }
                    }
                }

                _loadInventoryTask = null;

                IsLoading = false;
                CurrentProgress = 0;
                MaxProgress = -1;
            }
            finally { _loadSemaphore.Release(); }
        }

        if (inventory is null)
            throw new Exception("Failed to load inventory.");

        Loaded?.Invoke();
        return inventory;
    }

    private async Task<IEnumerable<InventoryItem>> LoadInventoryModernAsync(CancellationToken cancellationToken)
    {
        var loadItemsTcs = _loadInventoryItemsTcs
            = new TaskCompletionSource<IEnumerable<InventoryItem>>(TaskCreationOptions.RunContinuationsAsynchronously);
        cancellationToken.Register(() => loadItemsTcs.TrySetCanceled());

        _logger.LogDebug("Requesting inventory");
        Interceptor.Send(Out.RequestFurniInventory);

        return await loadItemsTcs.Task;
    }

    private async Task<IEnumerable<InventoryItem>> LoadInventoryOriginsAsync(
        int scanInterval, CancellationToken cancellationToken = default)
    {
        HashSet<InventoryItem> items = new(InventoryItemIdComparer.Default);
        bool cycled = false;
        int page = 1;

        while (!cycled)
        {
            _logger.LogDebug("Scanning inventory page {Page}.", page);
            Interceptor.Send(Xabbo.Messages.Shockwave.Out.GETSTRIP, (PacketContent)(page == 1 ? "new" : "next"));
            var packet = await Interceptor.ReceiveAsync(Xabbo.Messages.Shockwave.In.STRIPINFO_2,
                block: true, timeout: 3000, cancellationToken: cancellationToken);

            var pageItems = packet.Read<InventoryItem[]>();

            for (int i = 0; i < pageItems.Length; i++)
            {
                var item = pageItems[i];
                bool added = items.Add(item);
                if ((cycled && added) || (!added && !cycled && i > 0))
                    throw new Exception("The inventory changed while scanning.");
                if (!added && !cycled)
                {
                    _logger.LogDebug("Detected inventory page cycle, completing.");
                    cycled = true;
                }
            }

            if (cycled)
                break;

            _logger.LogDebug("Added {Count} items.", pageItems.Length);
            CurrentProgress = page;

            if (pageItems.Length < 9)
                break;

            await Task.Delay(scanInterval, cancellationToken);
            page++;
        }

        _logger.LogDebug("Scan loaded {Count} items.", items.Count);
        return items;
    }

    // On Origins:
    // Assume we have picked up the item if it is removed from the room and we are the room owner.
    // We need to detect special cases such as:
    // - A sticky note being deleted
    // - A gift being opened (not sure how to detect this yet)
    // But fortunately Origins uses furni identifiers so it is easier to detect this
    // without access to the furni data. Unforunately this means it must be hard-coded.

    private void OnFloorItemRemoved(FloorItemEventArgs e) => HandleRemovedFurni(e.Item);
    private void OnWallItemRemoved(WallItemEventArgs e) => HandleRemovedFurni(e.Item);

    private void HandleRemovedFurni(IFurni furni)
    {
        if (!Session.Is(ClientType.Origins) ||
            _inventory is not { } inventory ||
            !_roomManager.IsOwner ||
            furni.Identifier is not string identifier)
        {
            return;
        }

        if (furni is IWallItem { Identifier: "post.it" })
            return;

        // Add the item back into the inventory.
        if (furni is not IFloorItem { Size: { X: int width, Y: int length } })
        {
            width = 1;
            length = 1;
        }

        var item = new InventoryItem {
            Type = furni.Type,
            Id = furni.Id,
            ItemId = furni.Type is ItemType.Floor ? -furni.Id : furni.Id,
            Identifier = identifier,
            Size = furni is IFloorItem { Size: Point size } ? size : null,
            Data = new LegacyData() {
                Value = furni switch {
                    IFloorItem { Data.Value: string data } => data,
                    IWallItem { Data: string data } => data,
                    _ => ""
                }
            }
        };

        using var scope = _logger.MethodScope();

        if (inventory.TryAdd(item))
        {
            _logger.LogDebug("Added item #{ItemId}.", item.ItemId);
            ItemAdded?.Invoke(new InventoryItemEventArgs(item));
        }
        else
        {
            _logger.LogWarning("Failed to add item #{ItemId}.", item.ItemId);
        }
    }

    private void OnTradeCompleted(TradeCompletedEventArgs e)
    {
        if (!Session.Is(ClientType.Origins) ||
            _inventory is not { } inventory)
        {
            return;
        }

        // On Origins we must add trade items to our inventory after a successful trade.
        foreach (var item in e.PartnerOffer)
        {
            var inventoryItem = new InventoryItem {
                Type = item.Type,
                Id = item.Id,
                ItemId = item.ItemId,
                SlotId = item.SlotId,
                Identifier = item.Identifier,
                Size = item.Size,
                Data = new LegacyData() {
                    Value = item.Data.Value
                }
            };

            if (inventory.TryAdd(inventoryItem))
            {
                _logger.LogDebug("Added item #{ItemId}.", inventoryItem.ItemId);
                ItemAdded?.Invoke(new InventoryItemEventArgs(inventoryItem));
            }
            else
            {
                _logger.LogWarning("Failed to add item #{ItemId}.", inventoryItem.ItemId);
            }
        }
    }
}
