using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Response for <see cref="Outgoing.Modern.GetMarketplaceItemStatsMsg"/>.
/// </summary>
public sealed record MarketplaceItemStatsMsg(MarketplaceItemStats Stats) : IMessage<MarketplaceItemStatsMsg>
{
    static ClientType IMessage<MarketplaceItemStatsMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<MarketplaceItemStatsMsg>.Identifier => In.MarketplaceItemStats;
    static MarketplaceItemStatsMsg IParser<MarketplaceItemStatsMsg>.Parse(in PacketReader p) => new(p.Parse<MarketplaceItemStats>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Stats);
}
