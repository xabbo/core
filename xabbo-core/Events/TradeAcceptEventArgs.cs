using System;

namespace Xabbo.Core.Events
{
    public class TradeAcceptEventArgs : EventArgs
    {
        public RoomUser User { get; }
        public bool Accepted { get; }

        public TradeAcceptEventArgs(RoomUser user, bool accepted)
        {
            User = user;
            Accepted = accepted;
        }
    }
}
