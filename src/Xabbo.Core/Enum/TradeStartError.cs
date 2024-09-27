namespace Xabbo.Core;

/// <summary>
/// Represents an error received when failing to start a trade.
/// </summary>
public enum TradeStartError
{
    /// <summary>
    /// Trading is currently disabled hotel-wide.
    /// </summary>
    TradingDisabledHotel = 1,
    /// <summary>
    /// Trading is disabled for the current user.
    /// </summary>
    TradingDisabledAccount = 2,
    /// <summary>
    /// The target user currently has trading disabled.
    /// </summary>
    TradingDisabledPartner = 4,
    /// <summary>
    /// Trading is currently disabled in the room.
    /// </summary>
    TradingDisabledRoom = 6,
    /// <summary>
    /// The user is currently in a trade.
    /// </summary>
    OngoingTrade = 7,
    /// <summary>
    /// The target user is currently in a trade.
    /// </summary>
    PartnerOngoingTrade = 8
}
