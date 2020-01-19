using System;

namespace Xabbo.Core.Events
{
    public class RoomUserEffectEventArgs : RoomUserEventArgs
    {
        public int PreviousEffect { get; }

        public RoomUserEffectEventArgs(RoomUser user, int previousEffect)
            : base(user)
        {
            PreviousEffect = previousEffect;
        }
    }
}
