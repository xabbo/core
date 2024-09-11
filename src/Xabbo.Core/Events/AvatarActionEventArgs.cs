namespace Xabbo.Core.Events;

public sealed class AvatarActionEventArgs(IAvatar avatar, Actions action)
    : AvatarEventArgs(avatar)
{
    public Actions Action { get; } = action;
}
