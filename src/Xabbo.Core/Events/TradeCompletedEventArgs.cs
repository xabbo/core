namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.TradeManager.Completed"/> event.
/// </summary>
/// <param name="wasInitiator">Whether the user initiated the trade.</param>
/// <param name="self">The user's avatar instance.</param>
/// <param name="partner">The trade partner's avatar instance.</param>
/// <param name="selfOffer">The user's own offer.</param>
/// <param name="partnerOffer">The trade partner's offer.</param>
public sealed class TradeCompletedEventArgs(
    bool wasInitiator, IUser self, IUser partner,
    ITradeOffer selfOffer, ITradeOffer partnerOffer)
    : TradeUpdatedEventArgs(selfOffer, partnerOffer)
{
    /// <summary>
    /// Gets whether the current user initiated the trade.
    /// </summary>
    public bool WasInitiator { get; } = wasInitiator;

    /// <summary>
    /// Gets the user's own avatar instance.
    /// </summary>
    public IUser Self { get; } = self;

    /// <summary>
    /// Gets the trade partner's avatar instance.
    /// </summary>
    public IUser Partner { get; } = partner;
}
