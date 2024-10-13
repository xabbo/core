using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when offering items in a trade.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>.
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.AddItemsToTrade"/></item>
/// </list>
/// </summary>
public sealed class OfferTradeItemsMsg : List<Id>, IMessage<OfferTradeItemsMsg>
{
    public OfferTradeItemsMsg() { }
    public OfferTradeItemsMsg(IEnumerable<Id> itemIds) : base(itemIds) { }
    public OfferTradeItemsMsg(IEnumerable<IInventoryItem> items) : base(items.Select(item => item.ItemId)) { }

    static ClientType IMessage<OfferTradeItemsMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<OfferTradeItemsMsg>.Identifier => Out.AddItemsToTrade;
    static OfferTradeItemsMsg IParser<OfferTradeItemsMsg>.Parse(in PacketReader p) => new(p.ReadIdArray());
    void IComposer.Compose(in PacketWriter p) => p.WriteIdArray(this);
}