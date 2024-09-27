namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.AvatarAction"/> event.
/// </summary>
/// <param name="avatar">The avatar that performed the action.</param>
/// <param name="action">The action that the avatar performed.</param>
public sealed class AvatarActionEventArgs(IAvatar avatar, AvatarAction action)
    : AvatarEventArgs(avatar)
{
    /// <summary>
    /// The action that the avatar performed.
    /// </summary>
    public AvatarAction Action { get; } = action;
}
