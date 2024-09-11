namespace Xabbo.Core.Events;

public sealed class AvatarIdleEventArgs(IAvatar avatar, bool wasIdle)
    : AvatarEventArgs(avatar)
{
    public bool WasIdle { get; } = wasIdle;
}
