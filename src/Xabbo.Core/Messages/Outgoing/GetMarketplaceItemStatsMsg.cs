using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when requesting the marketplace statistics of an item.
/// <para/>
/// Request for <see cref="MarketplaceItemStatsMsg"/>. Returns a <see cref="MarketplaceItemStats"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.GetMarketplaceItemStats"/></item>
/// </list>
/// </summary>
/// <param name="Type">The type of the item to get marketplace statistics for.</param>
/// <param name="Kind">The kind of the item to get marketplace statistics for.</param>
public sealed record GetMarketplaceItemStatsMsg(ItemType Type, int Kind)
    : IRequestMessage<GetMarketplaceItemStatsMsg, MarketplaceItemStatsMsg, MarketplaceItemStats>
{
    static ClientType IMessage<GetMarketplaceItemStatsMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GetMarketplaceItemStatsMsg>.Identifier => Out.GetMarketplaceItemStats;
    bool IRequestFor<MarketplaceItemStatsMsg>.MatchResponse(MarketplaceItemStatsMsg msg) => msg.Stats.Type == Type && msg.Stats.Kind == Kind;
    MarketplaceItemStats IResponseData<MarketplaceItemStatsMsg, MarketplaceItemStats>.GetData(MarketplaceItemStatsMsg msg) => msg.Stats;
    static GetMarketplaceItemStatsMsg IParser<GetMarketplaceItemStatsMsg>.Parse(in PacketReader p) => new(
        Type: p.ReadInt() == 1 ? ItemType.Floor : ItemType.Wall,
        Kind: p.ReadInt()
    );
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Type is ItemType.Floor ? 1 : 2);
        p.WriteInt(Kind);
    }
}
