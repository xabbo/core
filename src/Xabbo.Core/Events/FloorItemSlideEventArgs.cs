namespace Xabbo.Core.Events;

public sealed class FloorItemSlideEventArgs(IFloorItem item, Tile previousTile, Id rollerId)
    : FloorItemEventArgs(item)
{
    public Tile PreviousTile { get; } = previousTile;
    public Id RollerId = rollerId;
}
