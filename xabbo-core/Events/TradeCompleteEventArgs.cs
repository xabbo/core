using System;

namespace Xabbo.Core.Events
{
    public class TradeCompleteEventArgs : EventArgs
    {
        public RoomUser Self { get; }
        public RoomUser Partner { get; }
        public TradeOffer SelfOffer { get; } 
        public TradeOffer PartnerOffer { get; }

        public TradeCompleteEventArgs(RoomUser self, RoomUser partner, TradeUpdate update)
        {
            Self = self;
            Partner = partner;
            SelfOffer = update.Self;
            PartnerOffer = update.Partner;
        }
    }
}
