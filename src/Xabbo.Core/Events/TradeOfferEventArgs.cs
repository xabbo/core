namespace Xabbo.Core.Events;

public class TradeOfferEventArgs(ITradeOffer ownOffer, ITradeOffer partnerOffer)
{
    /// <summary>
    /// Gets the user's offer.
    /// </summary>
    public ITradeOffer OwnOffer { get; } = ownOffer;

    /// <summary>
    /// Gets the trade partner's offer.
    /// </summary>
    public ITradeOffer PartnerOffer { get; } = partnerOffer;
}
