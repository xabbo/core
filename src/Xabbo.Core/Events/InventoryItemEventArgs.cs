namespace Xabbo.Core.Events;

public class InventoryItemEventArgs(IInventoryItem item)
{
    public IInventoryItem Item { get; } = item;
}
