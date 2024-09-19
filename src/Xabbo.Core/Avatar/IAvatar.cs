using Xabbo.Core.Game;

namespace Xabbo.Core;

/// <summary>
/// Represents an avatar in a room.
/// </summary>
public interface IAvatar : IFloorEntity
{
    /// <summary>
    /// Gets if the avatar has been removed from the room.
    /// </summary>
    bool IsRemoved { get; }

    /// <summary>
    /// Gets if the avatar is hidden client-side by the <see cref="RoomManager" />.
    /// </summary>
    bool IsHidden { get; }

    /// <summary>
    /// Gets the type of the avatar.
    /// </summary>
    AvatarType Type { get; }

    /// <summary>
    /// Gets the ID of the avatar.
    /// </summary>
    Id Id { get; }

    /// <summary>
    /// Gets the index of the avatar.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// Gets the name of the avatar.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the motto of the avatar.
    /// </summary>
    string Motto { get; }

    /// <summary>
    /// Gets the figure string of the avatar.
    /// </summary>
    string Figure { get; }

    /// <summary>
    /// Gets the X coordinate of the avatar.
    /// </summary>
    public int X => Location.X;

    /// <summary>
    /// Gets the Y coordinate of the avatar.
    /// </summary>
    public int Y => Location.Y;

    /// <summary>
    /// Gets the XY coordinates of the avatar.
    /// </summary>
    public Point XY => Location.XY;

    /// <summary>
    /// Gets the Z coordinate of the avatar.
    /// </summary>
    public float Z => Location.Z;

    /// <summary>
    /// Gets the current dance of the avatar.
    /// </summary>
    Dances Dance { get; }

    /// <summary>
    /// Gets if the avatar is idle or not.
    /// </summary>
    bool IsIdle { get; }

    /// <summary>
    /// Gets if the avatar is typing or not.
    /// </summary>
    bool IsTyping { get; }

    /// <summary>
    /// Gets the hand item the avatar is currently holding.
    /// </summary>
    int HandItem { get; }

    /// <summary>
    /// Gets the current effect of the avatar.
    /// </summary>
    int Effect { get; }

    /// <summary>
    /// Gets the current update of the avatar.
    /// </summary>
    IAvatarStatus? CurrentUpdate { get; }

    /// <summary>
    /// Gets the previous update of the avatar.
    /// </summary>
    IAvatarStatus? PreviousUpdate { get; }
}
