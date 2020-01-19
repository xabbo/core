using System;

namespace Xabbo.Core.Events
{
    public class RoomUserDanceEventArgs : RoomUserEventArgs
    {
        public int PreviousDance { get; }

        public RoomUserDanceEventArgs(RoomUser user, int previousDance)
            : base(user)
        {
            PreviousDance = previousDance;
        }

    }
}
