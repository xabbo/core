using System.Collections.Generic;
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Response for <see cref="Outgoing.Modern.GetMarketplaceOffersMsg"/>.
/// </summary>
public sealed record MarketplaceOffersMsg(List<MarketplaceOffer> Offers) : IMessage<MarketplaceOffersMsg>
{
    static ClientType IMessage<MarketplaceOffersMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<MarketplaceOffersMsg>.Identifier => In.MarketPlaceOffers;
    static MarketplaceOffersMsg IParser<MarketplaceOffersMsg>.Parse(in PacketReader p) => new([.. p.ParseArray<MarketplaceOffer>()]);
    void IComposer.Compose(in PacketWriter p) => p.ComposeArray(Offers);
}
