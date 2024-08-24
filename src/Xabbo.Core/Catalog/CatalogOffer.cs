using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class CatalogOffer : ICatalogOffer, IComposer, IParser<CatalogOffer>
{
    public int Id { get; set; }
    public CatalogPage? Page { get; set; }
    ICatalogPage? ICatalogOffer.Page => Page;
    public string FurniLine { get; set; } = string.Empty;
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
    public string PreviewImage { get; set; } = string.Empty;

    public CatalogOffer()
    {
        FurniLine = string.Empty;
        Products = new List<CatalogProduct>();
        PreviewImage = string.Empty;
    }

    protected CatalogOffer(in PacketReader p, bool fromCatalog)
    {
        Id = p.Read<int>();
        FurniLine = p.Read<string>();
        IsRentable = p.Read<bool>();
        PriceInCredits = p.Read<int>();
        PriceInActivityPoints = p.Read<int>();
        ActivityPointType = (ActivityPointType)p.Read<int>();
        PriceInSilver = p.Read<int>();
        CanPurchaseAsGift = p.Read<bool>();

        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
            Products.Add(p.Parse<CatalogProduct>());

        ClubLevel = p.Read<int>();
        CanPurchaseMultiple = p.Read<bool>();

        if (fromCatalog)
        {
            IsPet = p.Read<bool>();
            PreviewImage = p.Read<string>();
        }
    }

    public void Compose(in PacketWriter p, bool fromCatalog = true)
    {
        p.Write(Id);
        p.Write(FurniLine);
        p.Write(IsRentable);
        p.Write(PriceInCredits);
        p.Write(PriceInActivityPoints);
        p.Write((int)ActivityPointType);
        p.Write(PriceInSilver);
        p.Write(CanPurchaseAsGift);
        p.Write(Products);
        p.Write(ClubLevel);
        p.Write(CanPurchaseMultiple);

        if (fromCatalog)
        {
            p.Write(IsPet);
            p.Write(PreviewImage);
        }
    }

    void IComposer.Compose(in PacketWriter p) => Compose(in p, true);

    public static CatalogOffer Parse(in PacketReader p) => Parse(in p, true);
    public static CatalogOffer Parse(in PacketReader p, bool fromCatalog = true)
        => new(in p, fromCatalog);
}
