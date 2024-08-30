namespace Xabbo.Core;

public interface IMarketplaceOffer : IItem
{
    /// <summary>
    /// Gets the ID of the marketplace offer.
    /// </summary>
    Id OfferId { get; }

    /// <summary>
    /// Gets the current status of this marketplace offer.
    /// </summary>
    MarketplaceOfferStatus Status { get; }

    /// <summary>
    /// Gets the item data for the marketplace offer.
    /// </summary>
    IItemData Data { get; }

    /// <summary>
    /// Gets the price of the marketplace offer.
    /// </summary>
    int Price { get; }

    /// <summary>
    /// Gets the remaining time of this offer in minutes.
    /// </summary>
    int TimeRemaining { get; }

    /// <summary>
    /// Gets the average price for this item.
    /// </summary>
    int Average { get; }

    /// <summary>
    /// Gets the number of open offers for this item.
    /// Not available when loaded from the user's own marketplace offers.
    /// </summary>
    int Offers { get; }
}
