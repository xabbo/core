using System;

namespace Xabbo.Core.Events
{
    public class RoomUserEventArgs : EntityEventArgs
    {
        public RoomUser User => (RoomUser)Entity;

        public RoomUserEventArgs(RoomUser user)
            : base(user)
        { }
    }
}
