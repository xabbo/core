namespace Xabbo.Core;

/// <summary>
/// Represents an entity that occupies an area in a room.
/// </summary>
public interface IFloorEntity
{
    /// <summary>
    /// Gets the location of this entity.
    /// </summary>
    Tile Location { get; }

    /// <summary>
    /// Gets the direction of this entity.
    /// </summary>
    int Direction { get; }

    /// <summary>
    /// Gets the area occupied by this entity.
    /// </summary>
    Area Area { get; }
}
