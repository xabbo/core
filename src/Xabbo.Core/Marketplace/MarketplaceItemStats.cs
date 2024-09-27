using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IMarketplaceItemStats"/>
public sealed class MarketplaceItemStats : IMarketplaceItemStats, IParserComposer<MarketplaceItemStats>
{
    public Id Id => -1;
    public ItemType Type { get; set; }
    public int Kind { get; set; }

    public int AverageSalePrice { get; set; }
    public int OfferCount { get; set; }
    public int HistorySizeInDays { get; set; }
    public List<MarketplaceTradeInfo> TradeInfo { get; set; }
    IReadOnlyList<IMarketplaceTradeInfo> IMarketplaceItemStats.History => TradeInfo;

    string? IItem.Identifier => null;

    public MarketplaceItemStats()
    {
        TradeInfo = [];
    }

    private MarketplaceItemStats(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        AverageSalePrice = p.ReadInt();
        OfferCount = p.ReadInt();
        HistorySizeInDays = p.ReadInt();
        TradeInfo = [.. p.ParseArray<MarketplaceTradeInfo>()];

        int itemType = p.ReadInt();
        Type = itemType switch
        {
            1 => ItemType.Floor,
            2 => ItemType.Wall,
            _ => throw new FormatException($"Unknown item type: {itemType}.")
        };
        Kind = p.ReadInt();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.WriteInt(AverageSalePrice);
        p.WriteInt(OfferCount);
        p.WriteInt(HistorySizeInDays);

        p.ComposeArray(TradeInfo);

        p.WriteInt(Type switch
        {
            ItemType.Floor => 1,
            ItemType.Wall => 2,
            _ => throw new Exception($"Cannot write item type: {Type}."),
        });
        p.WriteInt(Kind);
    }

    public override string ToString() => $"{nameof(MarketplaceItemStats)}/{Type}:{Kind}";

    static MarketplaceItemStats IParser<MarketplaceItemStats>.Parse(in PacketReader p) => new(in p);
}
