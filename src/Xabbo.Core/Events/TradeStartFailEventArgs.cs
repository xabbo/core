namespace Xabbo.Core.Events;

public sealed class TradeStartFailEventArgs(int reason, string name)
{
    /// <summary>
    /// The reason that the trade failed to start.
    /// </summary>
    public int Reason { get; } = reason;
    /// <summary>
    /// The name of the user the trade failed to start with.
    /// </summary>
    public string Name { get; } = name;
}
