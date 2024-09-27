using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents an offer in the catalog.
/// </summary>
public interface ICatalogOffer
{
    ICatalogPage? Page { get; }

    /// <summary>
    /// The ID of the catalog page.
    /// </summary>
    int Id { get; }

    string FurniLine { get; }

    bool IsRentable { get; }

    /// <summary>
    /// The price of the offer in credits.
    /// </summary>
    int PriceInCredits { get; }

    /// <summary>
    /// The price of the offer in activity points.
    /// </summary>
    /// <remarks>
    /// The activity point type is specified by <see cref="ActivityPointType"/>.
    /// </remarks>
    int PriceInActivityPoints { get; }

    /// <summary>
    /// The type of activity points used to purchase the offer, if
    /// <see cref="PriceInActivityPoints"/> is greater than zero.
    /// </summary>
    ActivityPointType ActivityPointType { get; }

    /// <summary>
    /// Whether the offer can be purchased as a gift.
    /// </summary>
    bool CanPurchaseAsGift { get; }

    /// <summary>
    /// The list of products included in the offer.
    /// </summary>
    IReadOnlyList<ICatalogProduct> Products { get; }

    /// <summary>
    /// The club level required to purchase the offer.
    /// </summary>
    int ClubLevel { get; }

    /// <summary>
    /// Whether multiple offers can be purchased.
    /// </summary>
    bool CanPurchaseMultiple { get; }

    /// <summary>
    /// Whether the offer is a pet.
    /// </summary>
    bool IsPet { get; }

    string PreviewImage { get; }
}
