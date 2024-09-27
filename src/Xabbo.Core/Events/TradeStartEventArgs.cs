namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.TradeManager.Opened"/> event.
/// </summary>
/// <param name="isInitiator">Whether the current user initiated the trade.</param>
/// <param name="partner">The partner's avatar instance.</param>
public sealed class TradeStartEventArgs(bool isInitiator, IUser partner)
{
    /// <summary>
    /// Gets whether the current user initiated the trade.
    /// </summary>
    public bool IsInitiator { get; } = isInitiator;

    /// <summary>
    /// Gets the trade partner's avatar instance.
    /// </summary>
    public IUser Partner { get; } = partner;
}
