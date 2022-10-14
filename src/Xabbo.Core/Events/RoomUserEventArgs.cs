using System;

namespace Xabbo.Core.Events;

public class RoomUserEventArgs : EntityEventArgs
{
    public IRoomUser User => (IRoomUser)Entity;

    public RoomUserEventArgs(IRoomUser user)
        : base(user)
    { }
}
