using System.Collections.Generic;

namespace Xabbo.Core;

public interface ICatalogOffer
{
    ICatalogPage? Page { get; }

    int Id { get; }
    string FurniLine { get; }
    bool IsRentable { get; }
    int PriceInCredits { get; }
    int PriceInActivityPoints { get; }
    ActivityPointType ActivityPointType { get; }
    bool CanPurchaseAsGift { get; }
    IReadOnlyList<ICatalogProduct> Products { get; }
    int ClubLevel { get; }
    bool CanPurchaseMultiple { get; }
    bool IsPet { get; }
    string PreviewImage { get; }
}
