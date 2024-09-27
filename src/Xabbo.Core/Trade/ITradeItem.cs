namespace Xabbo.Core;

/// <summary>
/// Represents an item in a trade.
/// </summary>
public interface ITradeItem : IInventoryItem
{
    int CreationDay { get; }
    int CreationMonth { get; }
    int CreationYear { get; }
}
