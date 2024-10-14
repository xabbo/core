namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.TradeManager.Closed"/> event.
/// </summary>
/// <param name="user">The user that closed the trade.</param>
/// <param name="reason">The reason that the trade was closed.</param>
public sealed class TradeClosedEventArgs(IUser? user, int? reason)
{
    /// <summary>
    /// Gets the user that closed the trade.
    /// </summary>
    /// <remarks>
    /// Available on modern clients.
    /// </remarks>
    public IUser? User { get; } = user;

    /// <summary>
    /// Gets the reason that the trade was closed.
    /// </summary>
    /// <remarks>
    /// Available on modern clients.
    /// </remarks>
    public int? Reason { get; } = reason;
}
