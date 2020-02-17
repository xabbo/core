using System;

namespace Xabbo.Core.Events
{
    public class TradeStartEventArgs : EventArgs
    {
        public RoomUser Self { get; }
        public RoomUser Partner { get; }

        public TradeStartEventArgs(RoomUser self, RoomUser partner)
        {
            Self = self;
            Partner = partner;
        }
    }
}
