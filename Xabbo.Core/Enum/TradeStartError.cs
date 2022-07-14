using System;

namespace Xabbo.Core;

public enum TradeStartError
{
    TradingDisabledHotel = 1,
    TradingDisabledAccount = 2,
    TradingDisabledPartner = 4,
    TradingDisabledRoom = 6,
    OngoingTrade = 7,
    PartnerOngoingTrade = 8
}
