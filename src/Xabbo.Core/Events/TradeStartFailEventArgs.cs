using System;

namespace Xabbo.Core.Events;

public class TradeStartFailEventArgs : EventArgs
{
    /// <summary>
    /// The reason that the trade failed to start.
    /// </summary>
    public int Reason { get; }
    /// <summary>
    /// The name of the user the trade failed to start with.
    /// </summary>
    public string Name { get; }

    public TradeStartFailEventArgs(int reason, string name)
    {
        Reason = reason;
        Name = name;
    }
}
