using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents the marketplace statistics of an item.
/// </summary>
public interface IMarketplaceItemStats : IItem
{
    /// <summary>
    /// Gets the average sale price in the last week.
    /// </summary>
    int Average { get; }

    /// <summary>
    /// Gets the number of offers that are currently open.
    /// </summary>
    int Offers { get; }

    /// <summary>
    /// Gets the trading history data.
    /// </summary>
    IReadOnlyList<IMarketplaceTradeInfo> TradeInfo { get; }
}
