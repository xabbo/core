using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when searching the marketplace for offers.
/// <para/>
/// Request for <see cref="MarketplaceOffersMsg"/>. Returns an array of <see cref="MarketplaceOffer"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.GetMarketplaceOffers"/></item>
/// </list>
/// </summary>
/// <param name="Name">The name of the item to search for. May be left blank.</param>
/// <param name="MinPrice">The minimum inclusive offer price to search for. <c>-1</c> indicates no minimum.</param>
/// <param name="MaxPrice">The maximum inclusive offer price to search for. <c>-1</c> indicates no maximum.</param>
/// <param name="Sort">Specifies the order in which to return results.</param>
public sealed record SearchMarketplaceOffersMsg(
    string Name = "",
    int MinPrice = -1,
    int MaxPrice = -1,
    MarketplaceSortOrder Sort = MarketplaceSortOrder.HighestPrice
)
    : IRequestMessage<SearchMarketplaceOffersMsg, MarketplaceOffersMsg, MarketplaceOffer[]>
{
    static ClientType IMessage<SearchMarketplaceOffersMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<SearchMarketplaceOffersMsg>.Identifier => Out.GetMarketplaceOffers;

    MarketplaceOffer[] IResponseData<MarketplaceOffersMsg, MarketplaceOffer[]>.GetData(MarketplaceOffersMsg msg) => [.. msg.Offers];

    static SearchMarketplaceOffersMsg IParser<SearchMarketplaceOffersMsg>.Parse(in PacketReader p) => new(
        MinPrice: p.ReadInt(),
        MaxPrice: p.ReadInt(),
        Name: p.ReadString(),
        Sort: (MarketplaceSortOrder)p.ReadInt()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(MinPrice);
        p.WriteInt(MaxPrice);
        p.WriteString(Name);
        p.WriteInt((int)Sort);
    }
}
