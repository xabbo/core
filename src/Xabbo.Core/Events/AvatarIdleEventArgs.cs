namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.AvatarIdle"/> event.
/// </summary>
/// <param name="avatar">The avatar.</param>
/// <param name="wasIdle">Whether the avatar was previously idle.</param>
/// <remarks>
/// <see cref="AvatarEventArgs.Avatar"/> contains the updated state of the avatar.
/// </remarks>
public sealed class AvatarIdleEventArgs(IAvatar avatar, bool wasIdle)
    : AvatarEventArgs(avatar)
{
    /// <summary>
    /// Gets whether the avatar was previously idle.
    /// </summary>
    public bool WasIdle { get; } = wasIdle;
}
