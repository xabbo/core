using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a trade offer is updated.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.TradingItemList"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.TRADE_ITEMS"/></item>
/// </list>
/// </summary>
/// <param name="First">The first offer in the trade.</param>
/// <param name="Second">The second offer in the trade.</param>
public sealed record TradeOffersMsg(TradeOffer First, TradeOffer Second) : IMessage<TradeOffersMsg>
{
    static Identifier IMessage<TradeOffersMsg>.Identifier => In.TradingItemList;

    static TradeOffersMsg IParser<TradeOffersMsg>.Parse(in PacketReader p)
        => new(p.Parse<TradeOffer>(), p.Parse<TradeOffer>());

    void IComposer.Compose(in PacketWriter p)
    {
        p.Compose(First);
        p.Compose(Second);
    }
}