using System.Collections.Generic;

namespace Xabbo.Core;

public interface IMarketplaceItemStats : IItem
{
    /// <summary>
    /// Gets the average sale price in the last week.
    /// </summary>
    int Average { get; }

    /// <summary>
    /// Gets the number of currently open offers.
    /// </summary>
    int Offers { get; }

    /// <summary>
    /// Gets the trading history information.
    /// </summary>
    IReadOnlyList<IMarketplaceTradeInfo> TradeInfo { get; }
}
