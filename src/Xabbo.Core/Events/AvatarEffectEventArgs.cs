namespace Xabbo.Core.Events;

public sealed class AvatarEffectEventArgs(IAvatar avatar, int previousEffect)
    : AvatarEventArgs(avatar)
{
    public int PreviousEffect { get; } = previousEffect;
}
