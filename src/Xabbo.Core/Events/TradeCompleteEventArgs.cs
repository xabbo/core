namespace Xabbo.Core.Events;

public sealed class TradeCompleteEventArgs(
    bool wasTrader, IUser self, IUser partner,
    ITradeOffer ownOffer, ITradeOffer partnerOffer)
    : TradeOfferEventArgs(ownOffer, partnerOffer)
{
    public bool WasTrader { get; } = wasTrader;
    public IUser Self { get; } = self;
    public IUser Partner { get; } = partner;
}
