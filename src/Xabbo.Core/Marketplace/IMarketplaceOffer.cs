namespace Xabbo.Core;

/// <summary>
/// Represents an offer for an item in the marketplace.
/// </summary>
public interface IMarketplaceOffer : IItem
{
    /// <summary>
    /// Gets the ID of the marketplace offer.
    /// </summary>
    Id OfferId { get; }

    /// <summary>
    /// Gets the current status of the marketplace offer.
    /// </summary>
    MarketplaceOfferStatus Status { get; }

    /// <summary>
    /// Gets the item data of the marketplace offer.
    /// </summary>
    IItemData Data { get; }

    /// <summary>
    /// Gets the price of the marketplace offer.
    /// </summary>
    int Price { get; }

    /// <summary>
    /// Gets the remaining time of the offer in minutes.
    /// </summary>
    int MinutesRemaining { get; }

    /// <summary>
    /// Gets the current average price of the item.
    /// </summary>
    int Average { get; }

    /// <summary>
    /// Gets the number of open offers for the item.
    /// </summary>
    /// <remarks>
    /// This is not available when the offers are loaded from the user's own marketplace listings.
    /// </remarks>
    int Offers { get; }
}
