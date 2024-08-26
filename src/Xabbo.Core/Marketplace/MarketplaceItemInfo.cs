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
    public List<MarketplaceTradeInfo> TradeInfo { get; set; } = [];
    IReadOnlyList<IMarketplaceTradeInfo> IMarketplaceItemInfo.TradeInfo => TradeInfo;

    public MarketplaceItemInfo() { }

    private MarketplaceItemInfo(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        Average = p.Read<int>();
        Offers = p.Read<int>();
        p.Read<int>();
        TradeInfo = [..p.ParseArray<MarketplaceTradeInfo>()];

        int itemType = p.Read<int>();
        Type = itemType switch
        {
            1 => ItemType.Floor,
            2 => ItemType.Wall,
            _ => throw new FormatException($"Unknown item type: {itemType}.")
        };
        Kind = p.Read<int>();
    }

    public void Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

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

    public override string ToString() => $"{nameof(MarketplaceItemInfo)}/{Type}:{Kind}";

    public static MarketplaceItemInfo Parse(in PacketReader p) => new(in p);
}
