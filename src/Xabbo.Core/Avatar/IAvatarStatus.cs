namespace Xabbo.Core;

/// <summary>
/// Represents a status update of an avatar in a room.
/// </summary>
public interface IAvatarStatus
{
    /// <summary>
    /// The index of the avatar that the update is for.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// The updated location of the avatar.
    /// </summary>
    Tile Location { get; }

    /// <summary>
    /// The direction the avatar's head is facing.
    /// </summary>
    int HeadDirection { get; }

    /// <summary>
    /// The direction the avatar's body is facing.
    /// </summary>
    int Direction { get; }

    /// <summary>
    /// The stance of the avatar.
    /// </summary>
    AvatarStance Stance { get; }

    /// <summary>
    /// Whether the user has rights.
    /// </summary>
    bool IsController { get; }

    /// <summary>
    /// The rights level of the user.
    /// </summary>
    RightsLevel RightsLevel { get; }

    /// <summary>
    /// Whether the user is trading.
    /// </summary>
    bool IsTrading { get; }

    /// <summary>
    /// The tile that the avatar is currently moving towards.
    /// </summary>
    Tile? MovingTo { get; }

    /// <summary>
    /// Whether the avatar is sitting on the floor.
    /// </summary>
    bool SittingOnFloor { get; }

    /// <summary>
    /// The height of the sit or lay stance.
    /// </summary>
    float? StanceHeight { get; }

    /// <summary>
    /// The sign currently held by the avatar.
    /// </summary>
    AvatarSign Sign { get; }
}
