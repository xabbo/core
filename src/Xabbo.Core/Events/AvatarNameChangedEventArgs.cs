namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.AvatarNameChanged"/> event.
/// </summary>
/// <param name="avatar">The avatar.</param>
/// <param name="previousName">The previous name of the avatar.</param>
/// <remarks>
/// <see cref="AvatarEventArgs.Avatar"/> contains the updated state of the avatar.
/// </remarks>
public sealed class AvatarNameChangedEventArgs(IAvatar avatar, string previousName)
    : AvatarEventArgs(avatar)
{
    /// <summary>
    /// Gets the previous name of the avatar.
    /// </summary>
    public string PreviousName { get; } = previousName;
}
