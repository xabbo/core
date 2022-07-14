using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public class MarketplaceTradeInfo : IMarketplaceTradeInfo
{
    public int DayOffset { get; set; }
    public int AverageSalePrice { get; set; }
    public int TradeVolume { get; set; }

    public MarketplaceTradeInfo() { }

    protected MarketplaceTradeInfo(IReadOnlyPacket packet)
    {
        DayOffset = packet.ReadInt();
        AverageSalePrice = packet.ReadInt();
        TradeVolume = packet.ReadInt();
    }

    public static MarketplaceTradeInfo Parse(IReadOnlyPacket packet) => new MarketplaceTradeInfo(packet);
}
