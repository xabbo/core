namespace Xabbo.Core;

/// <summary>
/// Represents an entity that occupies a space in a room.
/// </summary>
public interface IFloorEntity
{
    /// <summary>
    /// Gets the location of the entity in the room.
    /// </summary>
    Tile Location { get; }

    /// <summary>
    /// Gets the direction the entity is facing.
    /// </summary>
    int Direction { get; }

    /// <summary>
    /// Gets the size of the entity, if it is available.
    /// </summary>
    Point? Size { get; }

    /// <summary>
    /// Gets the area occupied by the entity, if it is available.
    /// </summary>
    Area? Area { get; }
}
