namespace Xabbo.Core.Events;

public sealed class FloorItemUpdatedEventArgs(IFloorItem previous, IFloorItem updated)
    : FloorItemEventArgs(updated)
{
    public IFloorItem PreviousItem { get; } = previous;
}
