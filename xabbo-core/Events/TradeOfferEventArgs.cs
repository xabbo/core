using System;

namespace Xabbo.Core.Events
{
    public class TradeOfferEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the user who started the trade.
        /// </summary>
        public IRoomUser Trader { get; }
        
        /// <summary>
        /// Gets the user who was traded.
        /// </summary>
        public IRoomUser Tradee { get; }

        /// <summary>
        /// Gets the user's trade offer.
        /// </summary>
        public ITradeOffer OwnOffer { get; } 

        /// <summary>
        /// Gets the partner's trade offer.
        /// </summary>
        public ITradeOffer PartnerOffer { get; }

        /// <summary>
        /// Gets the user's entity information.
        /// </summary>
        public IRoomUser Self => OwnOffer.UserId == Trader.Id ? Trader : Tradee;

        /// <summary>
        /// Gets the partner's entity information.
        /// </summary>
        public IRoomUser Partner => PartnerOffer.UserId == Trader.Id ? Trader : Tradee;

        public TradeOfferEventArgs(IRoomUser trader, IRoomUser tradee, ITradeOffer ownOffer, ITradeOffer partnerOffer)
        {
            Trader = trader;
            Tradee = tradee;
            OwnOffer = ownOffer;
            PartnerOffer = partnerOffer;
        }
    }
}
