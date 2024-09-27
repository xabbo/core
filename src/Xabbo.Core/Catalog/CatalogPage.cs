using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="ICatalogPage"/>
public sealed class CatalogPage : ICatalogPage, IParserComposer<CatalogPage>
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
        LayoutCode = "";

        Images = [];
        Texts = [];
        Offers = [];
        FrontPageItems = [];
    }

    private CatalogPage(in PacketReader p)
    {
        Id = p.ReadInt();
        CatalogType = p.ReadString();
        LayoutCode = p.ReadString();

        Images = [.. p.ReadStringArray()];
        Texts = [.. p.ReadStringArray()];

        Offers = [.. p.ParseArray<CatalogOffer>()];
        foreach (var offer in Offers)
            offer.Page = this;

        OfferId = p.ReadInt();
        AcceptSeasonCurrencyAsCredits = p.ReadBool();

        FrontPageItems = p.Available > 0 ? [.. p.ParseArray<CatalogPageItem>()] : [];
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString(CatalogType);
        p.WriteString(LayoutCode);
        p.WriteStringArray(Images);
        p.WriteStringArray(Texts);
        p.ComposeArray(Offers);
        p.WriteInt(OfferId);
        p.WriteBool(AcceptSeasonCurrencyAsCredits);
        p.ComposeArray(FrontPageItems);
    }

    static CatalogPage IParser<CatalogPage>.Parse(in PacketReader p) => new(in p);
}