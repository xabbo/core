namespace Xabbo.Core.Events;

public sealed class AvatarHandItemEventArgs(IAvatar avatar, int previousItem)
    : AvatarEventArgs(avatar)
{
    public int PreviousItem { get; } = previousItem;
}
