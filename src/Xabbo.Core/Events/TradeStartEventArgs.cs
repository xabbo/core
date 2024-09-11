namespace Xabbo.Core.Events;

public sealed class TradeStartEventArgs(bool isTrader, IUser partner)
{
    /// <summary>
    /// Gets if the user was the one who initiated the trade.
    /// </summary>
    public bool IsTrader { get; } = isTrader;

    /// <summary>
    /// Gets the partner of the trade.
    /// </summary>
    public IUser Partner { get; } = partner;
}
