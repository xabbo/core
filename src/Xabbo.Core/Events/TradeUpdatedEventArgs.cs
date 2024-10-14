namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.TradeManager.Updated"/> event.
/// </summary>
/// <param name="selfOffer">The user's own offer.</param>
/// <param name="partnerOffer">The trade partner's offer.</param>
public class TradeUpdatedEventArgs(ITradeOffer selfOffer, ITradeOffer partnerOffer)
{
    /// <summary>
    /// Gets the user's own offer.
    /// </summary>
    public ITradeOffer SelfOffer { get; } = selfOffer;

    /// <summary>
    /// Gets the trade partner's offer.
    /// </summary>
    public ITradeOffer PartnerOffer { get; } = partnerOffer;
}
