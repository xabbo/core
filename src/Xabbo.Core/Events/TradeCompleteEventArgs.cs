using System;

namespace Xabbo.Core.Events;

public class TradeCompleteEventArgs : TradeOfferEventArgs
{
    public bool WasTrader { get; }
    public IRoomUser Self { get; }
    public IRoomUser Partner { get; }

    public TradeCompleteEventArgs(bool wasTrader, IRoomUser self, IRoomUser partner, ITradeOffer ownOffer, ITradeOffer partnerOffer)
        : base(ownOffer, partnerOffer)
    {
        WasTrader = wasTrader;
        Self = self;
        Partner = partner;
    }
}
