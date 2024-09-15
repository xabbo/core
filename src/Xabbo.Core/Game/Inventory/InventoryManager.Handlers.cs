using System.Linq;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Xabbo.Messages.Flash;
using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

partial class InventoryManager
{
    [InterceptIn(nameof(In.FurniListInvalidate))]
    private void HandleFurniListInvalidate(Intercept e)
    {
        Log.LogTrace("Inventory invalidated.");

        if (_inventory is not null)
        {
            _inventory.IsInvalidated = true;
        }

        Invalidated?.Invoke();
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
            Loaded?.Invoke();
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
            ItemAdded?.Invoke(new InventoryItemEventArgs(item));
        }
        else
        {
            Log.LogTrace("Updated inventory item {id}.", item.Id);
            ItemUpdated?.Invoke(new InventoryItemEventArgs(item));
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
            ItemRemoved?.Invoke(new InventoryItemEventArgs(item));
        }
        else
        {
            Log.LogWarning("Failed to find inventory item {id} to remove!", itemId);
        }
    }
}