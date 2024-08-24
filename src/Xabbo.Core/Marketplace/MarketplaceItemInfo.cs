using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class MarketplaceItemInfo : IMarketplaceItemInfo, IComposer, IParser<MarketplaceItemInfo>
{
    public Id Id => -1;
    public ItemType Type { get; set; }
    public int Kind { get; set; }

    public int Average { get; set; }
    public int Offers { get; set; }
    public List<MarketplaceTradeInfo> TradeInfo { get; set; }
    IReadOnlyList<IMarketplaceTradeInfo> IMarketplaceItemInfo.TradeInfo => TradeInfo;

    public MarketplaceItemInfo()
    {
        TradeInfo = [];
    }

    private MarketplaceItemInfo(in PacketReader p)
    {
        Average = p.Read<int>();
        Offers = p.Read<int>();
        p.Read<int>();
        int n = p.Read<Length>();
        TradeInfo = [];
        for (int i = 0; i < n; i++)
            TradeInfo.Add(p.Parse<MarketplaceTradeInfo>());

        int itemType = p.Read<int>();
        Type = itemType switch
        {
            1 => ItemType.Floor,
            2 => ItemType.Wall,
            _ => throw new FormatException($"Unknown item type: {itemType}.")
        };
        Kind = p.Read<int>();
    }

    public static MarketplaceItemInfo Parse(in PacketReader packet) => new(in packet);

    public override string ToString() => $"{nameof(MarketplaceItemInfo)}/{Type}:{Kind}";

    public void Compose(in PacketWriter p)
    {
        p.Write(Average);
        p.Write(Offers);
        p.Write(0); // ?

        p.Write<Length>(TradeInfo.Count);
        for (int i = 0; i < TradeInfo.Count; i++)
            p.Write(TradeInfo[i]);

        p.Write(Type switch {
           ItemType.Floor => 1,
           ItemType.Wall => 2,
           _ => throw new Exception($"Cannot write item type: {Type}."),
        });
        p.Write(Kind);
    }

}
