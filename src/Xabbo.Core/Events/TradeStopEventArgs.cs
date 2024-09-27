namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.TradeManager.Closed"/> event.
/// </summary>
/// <param name="user">The user that closed the trade.</param>
/// <param name="reason">The reason that the trade was closed.</param>
public sealed class TradeStopEventArgs(IUser user, int reason)
    : RoomUserEventArgs(user)
{
    /// <summary>
    /// Gets the reason that the trade was closed.
    /// </summary>
    public int Reason { get; } = reason;
}
