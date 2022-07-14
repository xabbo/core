using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class MarketplaceItemInfo : IMarketplaceItemInfo
{
    public long Id => -1;
    public ItemType Type { get; set; }
    public int Kind { get; set; }

    public int Average { get; set; }
    public int Offers { get; set; }
    public List<MarketplaceTradeInfo> TradeInfo { get; set; }
    IReadOnlyList<IMarketplaceTradeInfo> IMarketplaceItemInfo.TradeInfo => TradeInfo;

    public MarketplaceItemInfo()
    {
        TradeInfo = new List<MarketplaceTradeInfo>();
    }

    protected MarketplaceItemInfo(IReadOnlyPacket packet)
    {
        Average = packet.ReadInt();
        Offers = packet.ReadInt();
        packet.ReadInt();
        int n = packet.ReadInt();
        TradeInfo = new List<MarketplaceTradeInfo>();
        for (int i = 0; i < n; i++)
            TradeInfo.Add(MarketplaceTradeInfo.Parse(packet));

        int itemType = packet.ReadInt();
        Type = itemType switch
        {
            1 => ItemType.Floor,
            2 => ItemType.Wall,
            _ => throw new FormatException($"Unknown item type: {itemType}.")
        };
        Kind = packet.ReadInt();
    }

    public static MarketplaceItemInfo Parse(IReadOnlyPacket packet) => new MarketplaceItemInfo(packet);
}
