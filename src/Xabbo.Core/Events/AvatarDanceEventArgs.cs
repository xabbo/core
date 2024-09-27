namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.AvatarDance"/> event.
/// </summary>
/// <param name="avatar">The avatar.</param>
/// <param name="previousDance">The previous dance of the avatar.</param>
/// <remarks>
/// <see cref="AvatarEventArgs.Avatar"/> contains the updated state of the avatar.
/// </remarks>
public sealed class AvatarDanceEventArgs(IAvatar avatar, AvatarDance previousDance)
    : AvatarEventArgs(avatar)
{
    /// <summary>
    /// Gets the previous dance of the avatar.
    /// </summary>
    public AvatarDance PreviousDance { get; } = previousDance;
}
