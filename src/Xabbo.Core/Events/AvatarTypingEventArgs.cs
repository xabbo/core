namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.AvatarTyping"/> event.
/// </summary>
/// <param name="avatar">The avatar.</param>
/// <param name="wasTyping">Whether the avatar was previously typing.</param>
/// <remarks>
/// <see cref="AvatarEventArgs.Avatar"/> contains the updated state of the avatar.
/// </remarks>
public sealed class AvatarTypingEventArgs(IAvatar avatar, bool wasTyping)
    : AvatarEventArgs(avatar)
{
    /// <summary>
    /// Whether the avatar was previously typing.
    /// </summary>
    public bool WasTyping { get; } = wasTyping;
}
