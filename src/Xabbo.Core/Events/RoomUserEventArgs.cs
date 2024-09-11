namespace Xabbo.Core.Events;

public class RoomUserEventArgs(IUser user)
    : AvatarEventArgs(user)
{
    public IUser User { get; } = user;
}