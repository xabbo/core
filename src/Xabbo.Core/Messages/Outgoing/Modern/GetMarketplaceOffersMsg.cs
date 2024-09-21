using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Request for <see cref="MarketplaceOffersMsg"/>.
/// </summary>
public sealed record GetMarketplaceOffersMsg(
    string Search = "",
    int MinPrice = -1,
    int MaxPrice = -1,
    MarketplaceSortOrder Sort = MarketplaceSortOrder.HighestPrice
)
    : IRequestMessage<GetMarketplaceOffersMsg, MarketplaceOffersMsg, MarketplaceOffer[]>
{
    static Identifier IMessage<GetMarketplaceOffersMsg>.Identifier => Out.GetMarketplaceOffers;

    MarketplaceOffer[] IResponseData<MarketplaceOffersMsg, MarketplaceOffer[]>.GetData(MarketplaceOffersMsg msg) => [.. msg.Offers];

    static GetMarketplaceOffersMsg IParser<GetMarketplaceOffersMsg>.Parse(in PacketReader p) => new(
        MinPrice: p.ReadInt(),
        MaxPrice: p.ReadInt(),
        Search: p.ReadString(),
        Sort: (MarketplaceSortOrder)p.ReadInt()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(MinPrice);
        p.WriteInt(MaxPrice);
        p.WriteString(Search);
        p.WriteInt((int)Sort);
    }
}
