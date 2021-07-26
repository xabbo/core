using System;

namespace Xabbo.Core
{ 
    public interface IMarketplaceTradeInfo
    {
        int DayOffset { get; }
        int AverageSalePrice { get; }
        int TradeVolume { get; }
    }
}
