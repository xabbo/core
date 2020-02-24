using System;

namespace Xabbo.Core.Events
{
    public class TradeOfferEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the user who started the trade.
        /// </summary>
        public RoomUser Trader { get; }
        
        /// <summary>
        /// Gets the user who was traded.
        /// </summary>
        public RoomUser Tradee { get; }

        /// <summary>
        /// Gets the user's trade offer.
        /// </summary>
        public TradeOffer OwnOffer { get; } 

        /// <summary>
        /// Gets the partner's trade offer.
        /// </summary>
        public TradeOffer PartnerOffer { get; }

        /// <summary>
        /// Gets the user's entity information.
        /// </summary>
        public RoomUser Self => OwnOffer.UserId == Trader.Id ? Trader : Tradee;

        /// <summary>
        /// Gets the partner's entity information.
        /// </summary>
        public RoomUser Partner => PartnerOffer.UserId == Trader.Id ? Trader : Tradee;

        public TradeOfferEventArgs(RoomUser trader, RoomUser tradee, TradeOffer ownOffer, TradeOffer partnerOffer)
        {
            Trader = trader;
            Tradee = tradee;
            OwnOffer = ownOffer;
            PartnerOffer = partnerOffer;
        }
    }
}
