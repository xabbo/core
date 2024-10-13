using Xabbo.Messages;
using Xabbo.Messages.Shockwave;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when offering a single item in a trade.
/// <para/>
/// Supported clients: <see cref="ClientType.Origins"/>.
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Shockwave: <see cref="Out.TRADE_ADDITEM"/></item>
/// </list>
/// </summary>
/// <param name="ItemId">The ID of the item to offer.</param>
public sealed record OfferTradeItemMsg(Id ItemId) : IMessage<OfferTradeItemMsg>
{
    public OfferTradeItemMsg(IInventoryItem item) : this(item.ItemId) { }
    static ClientType IMessage<OfferTradeItemMsg>.SupportedClients => ClientType.Origins;
    static Identifier IMessage<OfferTradeItemMsg>.Identifier => Out.TRADE_ADDITEM;
    static OfferTradeItemMsg IParser<OfferTradeItemMsg>.Parse(in PacketReader p) => new((Id)p.ReadContent());
    void IComposer.Compose(in PacketWriter p) => p.WriteContent(ItemId.ToString());
}