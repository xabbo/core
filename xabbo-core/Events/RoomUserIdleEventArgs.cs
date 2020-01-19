using System;

namespace Xabbo.Core.Events
{
    public class RoomUserIdleEventArgs : RoomUserEventArgs
    {
        public bool WasIdle { get; }

        public RoomUserIdleEventArgs(RoomUser user, bool wasIdle)
            : base(user)
        {
            WasIdle = wasIdle;
        }
    }
}
