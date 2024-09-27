using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents the marketplace statistics of an item.
/// </summary>
public interface IMarketplaceItemStats : IItem
{
    /// <summary>
    /// The average sale price in the last 7 days.
    /// </summary>
    int AverageSalePrice { get; }

    /// <summary>
    /// The number of offers for this item kind that are currently open.
    /// </summary>
    int OfferCount { get; }

    /// <summary>
    /// The trading history data.
    /// </summary>
    IReadOnlyList<IMarketplaceTradeInfo> History { get; }
}
