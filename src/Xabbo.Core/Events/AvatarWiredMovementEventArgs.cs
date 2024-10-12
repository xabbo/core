namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Xabbo.Core.Game.RoomManager.AvatarWiredMovement"/> event.
/// </summary>
/// <param name="avatar">The avatar that was moved.</param>
/// <param name="previousLocation">The previous location of the avatar.</param>
/// <param name="previousDirection">The previous direction of the avatar.</param>
/// <param name="previousHeadDirection">The previous head direction of the avatar.</param>
/// <param name="movement">The wired movement that caused the event.</param>
public sealed class AvatarWiredMovementEventArgs(
    IAvatar avatar,
    Tile previousLocation,
    int previousDirection,
    int previousHeadDirection,
    AvatarWiredMovement movement
)
    : AvatarEventArgs(avatar)
{
    /// <summary>
    /// Gets the previous location of the avatar.
    /// </summary>
    public Tile PreviousLocation { get; } = previousLocation;

    /// <summary>
    /// Gets the previous direction of the avatar.
    /// </summary>
    public int PreviousDirection { get; } = previousDirection;

    /// <summary>
    /// Gets the previous direction of the avatar.
    /// </summary>
    public int PreviousHeadDirection { get; } = previousHeadDirection;

    /// <summary>
    /// Gets the wired movement that caused this event.
    /// </summary>
    public AvatarWiredMovement Movement { get; } = movement;
}