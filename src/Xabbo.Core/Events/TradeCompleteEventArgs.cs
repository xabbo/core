namespace Xabbo.Core.Events;

public sealed class TradeCompleteEventArgs(
    bool wasTrader, IRoomUser self, IRoomUser partner,
    ITradeOffer ownOffer, ITradeOffer partnerOffer)
    : TradeOfferEventArgs(ownOffer, partnerOffer)
{
    public bool WasTrader { get; } = wasTrader;
    public IRoomUser Self { get; } = self;
    public IRoomUser Partner { get; } = partner;
}
