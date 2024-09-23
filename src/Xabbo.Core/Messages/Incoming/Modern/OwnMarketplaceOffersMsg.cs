using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Response for <see cref="Outgoing.Modern.GetOwnMarketplaceOffersMsg"/>.
/// </summary>
public sealed record OwnMarketplaceOffersMsg(UserMarketplaceOffers Offers) : IMessage<OwnMarketplaceOffersMsg>
{
    static ClientType IMessage<OwnMarketplaceOffersMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<OwnMarketplaceOffersMsg>.Identifier => In.MarketPlaceOwnOffers;
    static OwnMarketplaceOffersMsg IParser<OwnMarketplaceOffersMsg>.Parse(in PacketReader p) => new(p.Parse<UserMarketplaceOffers>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Offers);
}
