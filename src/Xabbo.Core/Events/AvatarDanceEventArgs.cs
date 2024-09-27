namespace Xabbo.Core.Events;

public sealed class AvatarDanceEventArgs(IAvatar avatar, AvatarDance previousDance)
    : AvatarEventArgs(avatar)
{
    public AvatarDance PreviousDance { get; } = previousDance;
}
