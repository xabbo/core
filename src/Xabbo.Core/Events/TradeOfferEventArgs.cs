namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.TradeManager.Updated"/> event.
/// </summary>
/// <param name="ownOffer">The user's own offer.</param>
/// <param name="partnerOffer">The trade partner's offer.</param>
public class TradeOfferEventArgs(ITradeOffer ownOffer, ITradeOffer partnerOffer)
{
    /// <summary>
    /// Gets the user's own offer.
    /// </summary>
    public ITradeOffer OwnOffer { get; } = ownOffer;

    /// <summary>
    /// Gets the trade partner's offer.
    /// </summary>
    public ITradeOffer PartnerOffer { get; } = partnerOffer;
}
