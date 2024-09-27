using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="ICatalogOffer"/>
public class CatalogOffer : ICatalogOffer, IParserComposer<CatalogOffer>
{
    public int Id { get; set; }
    public CatalogPage? Page { get; set; }
    ICatalogPage? ICatalogOffer.Page => Page;
    public string FurniLine { get; set; } = "";
    public bool IsRentable { get; set; }
    public int PriceInCredits { get; set; }
    public int PriceInActivityPoints { get; set; }
    public ActivityPointType ActivityPointType { get; set; }
    public int PriceInSilver { get; set; }
    public bool CanPurchaseAsGift { get; set; }
    public List<CatalogProduct> Products { get; set; } = new();
    IReadOnlyList<ICatalogProduct> ICatalogOffer.Products => Products;
    public int ClubLevel { get; set; }
    public bool CanPurchaseMultiple { get; set; }
    public bool IsPet { get; set; }
    public string PreviewImage { get; set; } = "";

    public CatalogOffer()
    {
        FurniLine = "";
        Products = new List<CatalogProduct>();
        PreviewImage = "";
    }

    protected CatalogOffer(in PacketReader p)
    {
        Id = p.ReadInt();
        FurniLine = p.ReadString();
        IsRentable = p.ReadBool();
        PriceInCredits = p.ReadInt();
        PriceInActivityPoints = p.ReadInt();
        ActivityPointType = (ActivityPointType)p.ReadInt();
        PriceInSilver = p.ReadInt();
        CanPurchaseAsGift = p.ReadBool();
        Products = [.. p.ParseArray<CatalogProduct>()];
        ClubLevel = p.ReadInt();
        CanPurchaseMultiple = p.ReadBool();

        // if (fromCatalog)
        // {
        IsPet = p.ReadBool();
        PreviewImage = p.ReadString();
        // }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Id);
        p.WriteString(FurniLine);
        p.WriteBool(IsRentable);
        p.WriteInt(PriceInCredits);
        p.WriteInt(PriceInActivityPoints);
        p.WriteInt((int)ActivityPointType);
        p.WriteInt(PriceInSilver);
        p.WriteBool(CanPurchaseAsGift);
        p.ComposeArray(Products);
        p.WriteInt(ClubLevel);
        p.WriteBool(CanPurchaseMultiple);

        // if (fromCatalog)
        // {
        p.WriteBool(IsPet);
        p.WriteString(PreviewImage);
        // }
    }

    static CatalogOffer IParser<CatalogOffer>.Parse(in PacketReader p) => new(in p);
}
