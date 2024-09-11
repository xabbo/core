namespace Xabbo.Core.Events;

public class AvatarEventArgs(IAvatar avatar)
{
    public IAvatar Avatar => avatar;
}
