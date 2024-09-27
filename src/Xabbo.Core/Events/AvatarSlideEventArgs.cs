namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.AvatarSlide"/> event.
/// </summary>
/// <param name="avatar">The sliding avatar.</param>
/// <param name="previousLocation">The previous location of the avatar.</param>
/// <remarks>
/// <see cref="AvatarEventArgs.Avatar"/> contains the updated state of the avatar.
/// </remarks>
public sealed class AvatarSlideEventArgs(IAvatar avatar, Tile previousLocation)
    : AvatarEventArgs(avatar)
{
    /// <summary>
    /// Gets the previous locatin of the avatar.
    /// </summary>
    public Tile PreviousLocation { get; set; } = previousLocation;
}
