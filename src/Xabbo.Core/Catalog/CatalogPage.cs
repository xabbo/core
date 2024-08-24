using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class CatalogPage : ICatalogPage, IComposer, IParser<CatalogPage>
{
    public static CatalogPage Parse(in PacketReader packet) => new(in packet);

    public int Id { get; set; }
    public string CatalogType { get; set; }
    public string LayoutCode { get; set; }
    public List<string> Images { get; set; }
    IReadOnlyList<string> ICatalogPage.Images => Images;
    public List<string> Texts { get; set; }
    IReadOnlyList<string> ICatalogPage.Texts => Texts;
    public List<CatalogOffer> Offers { get; set; }
    IReadOnlyList<ICatalogOffer> ICatalogPage.Offers => Offers;
    public int OfferId { get; set; }
    public bool AcceptSeasonCurrencyAsCredits { get; set; }
    public List<CatalogPageItem> FrontPageItems { get; set; }
    IReadOnlyList<ICatalogPageItem> ICatalogPage.Data => FrontPageItems;

    public CatalogPage()
    {
        CatalogType =
        LayoutCode = string.Empty;

        Images = [];
        Texts = [];
        Offers = [];
        FrontPageItems = [];
    }

    private CatalogPage(in PacketReader packet)
    {
        Id = packet.Read<int>();
        CatalogType = packet.Read<string>();
        LayoutCode = packet.Read<string>();

        Images = [..packet.ReadArray<string>()];
        Texts = [..packet.ReadArray<string>()];

        Offers = [];
        int n = packet.Read<Length>();
        for (int i = 0; i < n; i++)
        {
            CatalogOffer offer = packet.Parse<CatalogOffer>();
            offer.Page = this;
            Offers.Add(offer);
        }

        OfferId = packet.Read<int>();
        AcceptSeasonCurrencyAsCredits = packet.Read<bool>();

        FrontPageItems = [];
        if (packet.Available > 0)
        {
            n = packet.Read<Length>();
            for (int i = 0; i < n; i++)
                FrontPageItems.Add(packet.Parse<CatalogPageItem>());
        }
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write(CatalogType);
        p.Write(LayoutCode);
        p.Write(Images);
        p.Write(Texts);
        p.Write(Offers);
        p.Write(OfferId);
        p.Write(AcceptSeasonCurrencyAsCredits);
        p.Write(FrontPageItems);
    }
}