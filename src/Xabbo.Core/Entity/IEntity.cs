using Xabbo.Messages;

using Xabbo.Core.Game;

namespace Xabbo.Core;

/// <summary>
/// Represents a living entity in a room.
/// </summary>
public interface IEntity : IFloorEntity
{
    /// <summary>
    /// Gets if the entity has been removed from the room.
    /// </summary>
    bool IsRemoved { get; }

    /// <summary>
    /// Gets if the entity is hidden client-side by the <see cref="RoomManager" />.
    /// </summary>
    bool IsHidden { get; }

    /// <summary>
    /// Gets the type of the entity.
    /// </summary>
    EntityType Type { get; }

    /// <summary>
    /// Gets the ID of the entity.
    /// </summary>
    Id Id { get; }

    /// <summary>
    /// Gets the index of the entity.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// Gets the name of the entity.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the motto of the entity.
    /// </summary>
    string Motto { get; }

    /// <summary>
    /// Gets the figure string of the entity.
    /// </summary>
    string Figure { get; }

    /// <summary>
    /// Gets the X coordinate of the entity.
    /// </summary>
    public int X => Location.X;

    /// <summary>
    /// Gets the Y coordinate of the entity.
    /// </summary>
    public int Y => Location.Y;

    /// <summary>
    /// Gets the XY coordinates of the entity.
    /// </summary>
    public Point XY => Location.XY;

    /// <summary>
    /// Gets the Z coordinate of the entity.
    /// </summary>
    public float Z => Location.Z;

    /// <summary>
    /// Gets the current dance of the entity.
    /// </summary>
    Dances Dance { get; }

    /// <summary>
    /// Gets if the entity is idle or not.
    /// </summary>
    bool IsIdle { get; }

    /// <summary>
    /// Gets if the entity is typing or not.
    /// </summary>
    bool IsTyping { get; }

    /// <summary>
    /// Gets the hand item the entity is currently holding.
    /// </summary>
    int HandItem { get; }

    /// <summary>
    /// Gets the current effect of the entity.
    /// </summary>
    int Effect { get; }

    /// <summary>
    /// Gets the current update of the entity.
    /// </summary>
    IEntityStatusUpdate? CurrentUpdate { get; }

    /// <summary>
    /// Gets the previous update of the entity.
    /// </summary>
    IEntityStatusUpdate? PreviousUpdate { get; }
}
