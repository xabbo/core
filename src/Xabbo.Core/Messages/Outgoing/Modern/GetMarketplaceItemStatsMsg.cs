using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Request for <see cref="MarketplaceItemStatsMsg"/>.
/// </summary>
public sealed record GetMarketplaceItemStatsMsg(ItemType Type, int Kind)
    : IRequestMessage<GetMarketplaceItemStatsMsg, MarketplaceItemStatsMsg, MarketplaceItemStats>
{
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
