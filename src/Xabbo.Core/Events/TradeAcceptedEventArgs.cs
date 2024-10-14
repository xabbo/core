namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.TradeManager.Accepted"/> event.
/// </summary>
/// <param name="user">The user who accepted or unaccepted.</param>
/// <param name="accepted">Whether the user accepted.</param>
public sealed class TradeAcceptedEventArgs(IUser user, bool accepted)
    : RoomUserEventArgs(user)
{
    /// <summary>
    /// Gets whether the user accepted the trade.
    /// </summary>
    public bool Accepted { get; } = accepted;
}
