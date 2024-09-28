using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received after requesting the user's own marketplace offers.
/// <para/>
/// Response for <see cref="Outgoing.GetOwnMarketplaceOffersMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.MarketPlaceOwnOffers"/></item>
/// </list>
/// </summary>
public sealed record OwnMarketplaceOffersMsg(UserMarketplaceOffers Offers) : IMessage<OwnMarketplaceOffersMsg>
{
    static ClientType IMessage<OwnMarketplaceOffersMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<OwnMarketplaceOffersMsg>.Identifier => In.MarketPlaceOwnOffers;
    static OwnMarketplaceOffersMsg IParser<OwnMarketplaceOffersMsg>.Parse(in PacketReader p) => new(p.Parse<UserMarketplaceOffers>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Offers);
}
