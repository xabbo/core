namespace Xabbo.Core.Events;

public class FloorItemEventArgs(IFloorItem item)
{
    public IFloorItem Item { get; } = item;
}
