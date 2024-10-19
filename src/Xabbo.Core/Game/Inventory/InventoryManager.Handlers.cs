using System.Linq;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Xabbo.Messages.Flash;
using Xabbo.Core.Events;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Game;

partial class InventoryManager
{
    [Intercept]
    private void HandleInventoryInvalidated(InventoryInvalidatedMsg _)
    {
        _logger.LogDebug("Inventory invalidated.");

        if (_inventory is not null)
        {
            _inventory.IsInvalidated = true;
        }

        Invalidated?.Invoke();
    }

    [Intercept(ClientType.Modern)]
    [InterceptIn(nameof(In.FurniList))]
    private void HandleFurniList(Intercept e)
    {
        _loadSemaphore.Wait();
        try
        {
            if (_loadInventoryItemsTcs is not { } loadTcs)
                return;

            InventoryFragment fragment = e.Packet.Read<InventoryFragment>();

            e.Block();

            if (fragment.Index == 0)
            {
                _logger.LogDebug("Resetting inventory load state.");

                _currentFragmentIndex = 0;
                _totalFragments = fragment.Total;
                _fragments.Clear();
            }
            else if (_currentFragmentIndex != fragment.Index || _totalFragments != fragment.Total)
            {
                _logger.LogWarning(
                    "Inventory load state mismatch!"
                    + " Expected {expectedIndex}/{expectedTotal};"
                    + " received {actualIndex}/{actualTotal} (index/total).",
                    _currentFragmentIndex, _totalFragments,
                    fragment.Index, fragment.Total
                );
                return;
            }

            _logger.LogDebug("Received inventory fragment {FragmentNumber} of {TotalFragments}.", fragment.Index + 1, fragment.Total);

            _currentFragmentIndex++;
            _fragments.Add(fragment);

            CurrentProgress = _currentFragmentIndex;
            MaxProgress = _totalFragments;

            if (fragment.Index == (fragment.Total - 1))
            {
                _currentFragmentIndex = 0;
                _totalFragments = 0;
                _logger.LogDebug("All inventory fragments received.");
                loadTcs.TrySetResult(_fragments.SelectMany(fragment => (ICollection<InventoryItem>)fragment));
            }
        }
        finally { _loadSemaphore.Release(); }
    }

    [Intercept]
    private void HandleInventoryItemAddedOrUpdated(InventoryItemAddedOrUpdatedMsg msg)
    {
        if (_inventory is not { } inventory) return;

        inventory.AddOrUpdate(msg.Item, out bool added);

        if (added)
        {
            _logger.LogDebug("Added inventory item #{ItemId}.", msg.Item.ItemId);
            ItemAdded?.Invoke(new InventoryItemEventArgs(msg.Item));
        }
        else
        {
            _logger.LogDebug("Updated inventory item #{ItemId}.", msg.Item.ItemId);
            ItemUpdated?.Invoke(new InventoryItemEventArgs(msg.Item));
        }
    }

    [Intercept]
    private void HandleInventoryItemRemoved(InventoryItemRemovedMsg msg)
    {
        if (_inventory is not { } inventory) return;

        if (inventory.TryRemove(msg.ItemId, out InventoryItem? item))
        {
            _logger.LogDebug("Inventory item #{ItemId} removed.", msg.ItemId);
            ItemRemoved?.Invoke(new InventoryItemEventArgs(item));
        }
        else
        {
            _logger.LogWarning("Failed to find inventory item #{ItemId} to remove.", msg.ItemId);
        }
    }
}