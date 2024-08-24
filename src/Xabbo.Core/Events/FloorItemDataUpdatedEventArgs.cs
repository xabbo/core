namespace Xabbo.Core.Events;

public sealed class FloorItemDataUpdatedEventArgs(IFloorItem item, IItemData previousData)
    : FloorItemEventArgs(item)
{
    public IItemData PreviousData { get; } = previousData;
}
