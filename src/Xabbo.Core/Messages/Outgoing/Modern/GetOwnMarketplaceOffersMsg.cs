using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Request for <see cref="OwnMarketplaceOffersMsg"/>.
/// </summary>
public sealed record GetOwnMarketplaceOffersMsg : IRequestMessage<GetOwnMarketplaceOffersMsg, OwnMarketplaceOffersMsg, UserMarketplaceOffers>
{
    static ClientType IMessage<GetOwnMarketplaceOffersMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GetOwnMarketplaceOffersMsg>.Identifier => Out.GetMarketplaceOwnOffers;
    UserMarketplaceOffers IResponseData<OwnMarketplaceOffersMsg, UserMarketplaceOffers>.GetData(OwnMarketplaceOffersMsg msg) => msg.Offers;
    static GetOwnMarketplaceOffersMsg IParser<GetOwnMarketplaceOffersMsg>.Parse(in PacketReader p) => new();
    void IComposer.Compose(in PacketWriter p) { }
}
