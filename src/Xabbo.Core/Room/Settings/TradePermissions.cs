using System;

namespace Xabbo.Core;

/// <summary>
/// Represents a room's trading permissions.
/// </summary>
public enum TradePermissions
{
    None = -1,
    /// <summary>
    /// Trading is not allowed.
    /// </summary>
    NotAllowed = 0,
    /// <summary>
    /// Only rights holders may initiate a trade.
    /// </summary>
    RightsHolders = 1,
    /// <summary>
    /// Trading is allowed.
    /// </summary>
    Allowed = 2
}

public static partial class XabboEnumExtensions
{
    public static string ToFriendlyName(this TradePermissions tradePermissions)
    {
        switch (tradePermissions)
        {
            case TradePermissions.NotAllowed: return "Not allowed";
            case TradePermissions.RightsHolders: return "Rights holders";
            case TradePermissions.Allowed: return "Allowed";
            default: return "Unknown";
        }
    }
}
