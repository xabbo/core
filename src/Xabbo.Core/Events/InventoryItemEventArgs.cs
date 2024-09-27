namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the
/// <see cref="Game.InventoryManager.ItemAdded"/>,
/// <see cref="Game.InventoryManager.ItemUpdated"/>, or
/// <see cref="Game.InventoryManager.ItemRemoved"/> event.
/// </summary>
/// <param name="item">The item involved in the event.</param>
public class InventoryItemEventArgs(IInventoryItem item)
{
    /// <summary>
    /// Gets the inventory item involved in the event.
    /// </summary>
    public IInventoryItem Item { get; } = item;
}
