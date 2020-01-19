using System;

namespace Xabbo.Core.Events
{
    public class RoomUserActionEventArgs : RoomUserEventArgs
    {
        public Actions Action { get; }

        public RoomUserActionEventArgs(RoomUser user, Actions action)
            :  base(user)
        {
            Action = action;
        }
    }
}
