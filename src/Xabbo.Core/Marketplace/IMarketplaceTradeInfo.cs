namespace Xabbo.Core;

/// <summary>
/// Represents a single data point containing the daily trading history of an item.
/// </summary>
public interface IMarketplaceTradeInfo
{
    /// <summary>
    /// Gets the negative offset in days from the day the information was requested.
    /// </summary>
    int DayOffset { get; }
    /// <summary>
    /// Gets the average sale price of the day.
    /// </summary>
    int AverageSalePrice { get; }
    /// <summary>
    /// Gets the total trade volume of the day.
    /// </summary>
    int TradeVolume { get; }
}
