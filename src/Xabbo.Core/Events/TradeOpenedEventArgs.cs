namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.TradeManager.Opened"/> event.
/// </summary>
/// <param name="isInitiator">Whether the current user initiated the trade.</param>
/// <param name="self">The user's own avatar instance.</param>
/// <param name="partner">The partner's avatar instance.</param>
public sealed class TradeOpenedEventArgs(bool isInitiator, IUser self, IUser partner)
{
    /// <summary>
    /// Gets whether the current user initiated the trade.
    /// </summary>
    public bool IsInitiator { get; } = isInitiator;

    /// <summary>
    /// Gets the user's own avatar instance.
    /// </summary>
    public IUser Self { get; } = self;

    /// <summary>
    /// Gets the trade partner's avatar instance.
    /// </summary>
    public IUser Partner { get; } = partner;
}
