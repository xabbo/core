using System;

namespace Xabbo.Core.Events;

public class TradeOfferEventArgs : EventArgs
{
    /// <summary>
    /// Gets the user's offer.
    /// </summary>
    public ITradeOffer OwnOffer { get; } 

    /// <summary>
    /// Gets the trade partner's offer.
    /// </summary>
    public ITradeOffer PartnerOffer { get; }

    public TradeOfferEventArgs(ITradeOffer ownOffer, ITradeOffer partnerOffer)
    {
        OwnOffer = ownOffer;
        PartnerOffer = partnerOffer;
    }
}
