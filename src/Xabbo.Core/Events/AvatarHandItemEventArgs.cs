namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.AvatarHandItem"/> event.
/// </summary>
/// <param name="avatar">The avatar.</param>
/// <param name="previousItem">The previous item that the avatar was holding.</param>
/// <remarks>
/// <see cref="AvatarEventArgs.Avatar"/> contains the updated state of the avatar.
/// </remarks>
public sealed class AvatarHandItemEventArgs(IAvatar avatar, int previousItem)
    : AvatarEventArgs(avatar)
{
    /// <summary>
    /// Gets the previous item that the avatar was holding.
    /// </summary>
    public int PreviousItem { get; } = previousItem;
}
