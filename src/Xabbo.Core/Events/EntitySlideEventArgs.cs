namespace Xabbo.Core.Events;

public sealed class EntitySlideEventArgs(IEntity entity, Tile previousTile)
    : EntityEventArgs(entity)
{
    public Tile PreviousTile { get; set; } = previousTile;
}
