using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received after requesting the marketplace statistics of an item.
/// <para/>
/// Response for <see cref="Outgoing.GetMarketplaceItemStatsMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.MarketplaceItemStats"/></item>
/// </list>
/// </summary>
/// <param name="Stats">The marketplace item statistics.</param>
public sealed record MarketplaceItemStatsMsg(MarketplaceItemStats Stats) : IMessage<MarketplaceItemStatsMsg>
{
    static ClientType IMessage<MarketplaceItemStatsMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<MarketplaceItemStatsMsg>.Identifier => In.MarketplaceItemStats;
    static MarketplaceItemStatsMsg IParser<MarketplaceItemStatsMsg>.Parse(in PacketReader p) => new(p.Parse<MarketplaceItemStats>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Stats);
}
