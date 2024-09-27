namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.RoomManager.AvatarEffect"/> event.
/// </summary>
/// <param name="avatar">The avatar.</param>
/// <param name="previousEffect">The previous effect of the avatar.</param>
/// <remarks>
/// <see cref="AvatarEventArgs.Avatar"/> contains the updated state of the avatar.
/// </remarks>
public sealed class AvatarEffectEventArgs(IAvatar avatar, int previousEffect)
    : AvatarEventArgs(avatar)
{
    /// <summary>
    /// Gets the previous effect of the avatar.
    /// </summary>
    public int PreviousEffect { get; } = previousEffect;
}
