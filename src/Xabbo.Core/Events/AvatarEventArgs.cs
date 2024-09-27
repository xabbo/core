namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the
/// <see cref="Game.RoomManager.AvatarAdded"/>,
/// <see cref="Game.RoomManager.AvatarUpdated"/>, or
/// <see cref="Game.RoomManager.AvatarRemoved"/> event.
/// </summary>
/// <param name="avatar">The avatar involved in the event.</param>
public class AvatarEventArgs(IAvatar avatar)
{
    /// <summary>
    /// Gets the avatar involved in the event.
    /// </summary>
    public IAvatar Avatar => avatar;
}
