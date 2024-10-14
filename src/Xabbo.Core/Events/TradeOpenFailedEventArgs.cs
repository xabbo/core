namespace Xabbo.Core.Events;

/// <summary>
/// Provides data for the <see cref="Game.TradeManager.OpenFailed"/> event.
/// </summary>
/// <param name="reason">The reason that the trade failed to start.</param>
/// <param name="name">The name of the user that the trade failed to start with.</param>
public sealed class TradeOpenFailedEventArgs(int reason, string name)
{
    /// <summary>
    /// Gets reason that the trade failed to start.
    /// </summary>
    public int Reason { get; } = reason;

    /// <summary>
    /// Gets name of the user that the trade failed to start with.
    /// </summary>
    public string Name { get; } = name;
}
