using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class CatalogPage : ICatalogPage, IComposer, IParser<CatalogPage>
{
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

    private CatalogPage(in PacketReader p)
    {
        Id = p.Read<int>();
        CatalogType = p.Read<string>();
        LayoutCode = p.Read<string>();

        Images = [..p.ReadArray<string>()];
        Texts = [..p.ReadArray<string>()];

        Offers = [];
        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
        {
            CatalogOffer offer = p.Parse<CatalogOffer>();
            offer.Page = this;
            Offers.Add(offer);
        }

        OfferId = p.Read<int>();
        AcceptSeasonCurrencyAsCredits = p.Read<bool>();

        FrontPageItems = [];
        if (p.Available > 0)
        {
            n = p.Read<Length>();
            for (int i = 0; i < n; i++)
                FrontPageItems.Add(p.Parse<CatalogPageItem>());
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

    public static CatalogPage Parse(in PacketReader p) => new(in p);
}