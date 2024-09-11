namespace Xabbo.Core.Events;

public sealed class AvatarDanceEventArgs(IAvatar avatar, Dances previousDance)
    : AvatarEventArgs(avatar)
{
    public Dances PreviousDance { get; } = previousDance;
}
