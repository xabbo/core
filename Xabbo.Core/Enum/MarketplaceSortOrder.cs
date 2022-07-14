using System;

namespace Xabbo.Core;

/// <summary>
/// Defines the sort orders available when searching the marketplace.
/// </summary>
public enum MarketplaceSortOrder
{
    /// <summary>
    /// Sorts results by highest price first.
    /// </summary>
    HighestPrice = 1,
    /// <summary>
    /// Sorts results by lowest price first.
    /// </summary>
    LowestPrice = 2,
    /// <summary>
    /// Sorts results by highest volume of trades first.
    /// </summary>
    MostTrades = 3,
    /// <summary>
    /// Sorts results by lowest volume of trades first.
    /// </summary>
    LeastTrades = 4,
    /// <summary>
    /// Sorts results by most number of offers first.
    /// </summary>
    MostOffers = 5,
    /// <summary>
    /// Sorts results by least number of offers first.
    /// </summary>
    LeastOffers = 6
}
