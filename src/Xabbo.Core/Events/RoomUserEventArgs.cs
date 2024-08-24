namespace Xabbo.Core.Events;

public class RoomUserEventArgs(IRoomUser user)
    : EntityEventArgs(user)
{
    public IRoomUser User { get; } = user;
}