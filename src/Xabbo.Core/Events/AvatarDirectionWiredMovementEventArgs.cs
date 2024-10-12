namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Xabbo.Core.Game.RoomManager.AvatarDirectionWiredMovement"/> event.
/// </summary>
/// <param name="avatar">The avatar that was rotated.</param>
/// <param name="previousDirection">The previous direction of the avatar.</param>
/// <param name="previousHeadDirection">The previous head direction of the avatar.</param>
/// <param name="movement">The wired movement that caused the event.</param>
public sealed class AvatarDirectionWiredMovementEventArgs(
    IAvatar avatar,
    int previousDirection,
    int previousHeadDirection,
    AvatarDirectionWiredMovement movement
)
    : AvatarEventArgs(avatar)
{
    /// <summary>
    /// Gets the previous direction of the avatar.
    /// </summary>
    public int PreviousDirection { get; } = previousDirection;

    /// <summary>
    /// Gets the previous head direction of the avatar.
    /// </summary>
    public int PreviousHeadDirection { get; } = previousHeadDirection;

    /// <summary>
    /// Gets the wired movement that caused this event.
    /// </summary>
    public AvatarDirectionWiredMovement Movement { get; } = movement;
}