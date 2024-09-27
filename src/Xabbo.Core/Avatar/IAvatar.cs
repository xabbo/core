using Xabbo.Core.Game;

namespace Xabbo.Core;

/// <summary>
/// Represents an avatar in a room.
/// </summary>
public interface IAvatar : IFloorEntity
{
    /// <summary>
    /// Whether the avatar has been removed from the room.
    /// </summary>
    bool IsRemoved { get; }

    /// <summary>
    /// Whether the avatar is hidden client-side by the <see cref="RoomManager" />.
    /// </summary>
    bool IsHidden { get; }

    /// <summary>
    /// The type of the avatar.
    /// </summary>
    AvatarType Type { get; }

    /// <summary>
    /// The ID of the avatar.
    /// </summary>
    Id Id { get; }

    /// <summary>
    /// The index of the avatar.
    /// </summary>
    /// <remarks>
    /// Each avatar is assigned a temporary incremental index number upon entering the room.
    /// </remarks>
    int Index { get; }

    /// <summary>
    /// The name of the avatar.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The motto of the avatar.
    /// </summary>
    string Motto { get; }

    /// <summary>
    /// The figure string of the avatar.
    /// </summary>
    string Figure { get; }

    /// <summary>
    /// The X coordinate of the avatar.
    /// </summary>
    public int X => Location.X;

    /// <summary>
    /// The Y coordinate of the avatar.
    /// </summary>
    public int Y => Location.Y;

    /// <summary>
    /// The XY coordinates of the avatar.
    /// </summary>
    public Point XY => Location.XY;

    /// <summary>
    /// The Z coordinate of the avatar.
    /// </summary>
    public float Z => Location.Z;

    /// <summary>
    /// The current dance of the avatar.
    /// </summary>
    AvatarDance Dance { get; }

    /// <summary>
    /// Whether the avatar is idle or not.
    /// </summary>
    bool IsIdle { get; }

    /// <summary>
    /// Whether the avatar is typing or not.
    /// </summary>
    bool IsTyping { get; }

    /// <summary>
    /// The ID of the hand item that the avatar is currently holding.
    /// </summary>
    int HandItem { get; }

    /// <summary>
    /// The current effect of the avatar.
    /// </summary>
    int Effect { get; }

    /// <summary>
    /// The current update of the avatar.
    /// </summary>
    IAvatarStatus? CurrentUpdate { get; }

    /// <summary>
    /// The previous update of the avatar.
    /// </summary>
    IAvatarStatus? PreviousUpdate { get; }
}
