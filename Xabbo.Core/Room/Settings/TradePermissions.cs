using System;

namespace Xabbo.Core;

public enum TradePermissions
{
    NotAllowed = 0,
    RightsHolders = 1,
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
