namespace Xabbo.Core.Events;

public class RoomUserEventArgs(IRoomUser user)
    : AvatarEventArgs(user)
{
    public IRoomUser User { get; } = user;
}