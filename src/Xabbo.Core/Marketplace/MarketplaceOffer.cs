using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IMarketplaceOffer"/>
public sealed class MarketplaceOffer : IMarketplaceOffer, IParserComposer<MarketplaceOffer>
{
    public Id Id => -1;
    public Id OfferId { get; set; }
    public MarketplaceOfferStatus Status { get; set; }
    public ItemType Type { get; set; }
    public int Kind { get; set; }
    public ItemData Data { get; set; }
    IItemData IMarketplaceOffer.Data => Data;
    public int Price { get; set; }
    public int MinutesRemaining { get; set; }
    public int Average { get; set; }
    public int Offers { get; set; }

    string? IItem.Identifier => null;

    public MarketplaceOffer()
    {
        Data = new LegacyData();
    }

    private MarketplaceOffer(in PacketReader p, bool hasOfferCount)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        OfferId = p.ReadId();
        Status = (MarketplaceOfferStatus)p.ReadInt();

        int itemType = p.ReadInt();
        switch (itemType)
        {
            case 1:
                Type = ItemType.Floor;
                Kind = p.ReadInt();
                Data = p.Parse<ItemData>();
                break;
            case 2:
                Type = ItemType.Wall;
                Kind = p.ReadInt();
                Data = new LegacyData() { Value = p.ReadString() };
                break;
            case 3:
                Type = ItemType.Floor;
                Kind = p.ReadInt();
                Data = new LegacyData()
                {
                    Flags = ItemDataFlags.IsLimitedRare,
                    UniqueSerialNumber = p.ReadInt(),
                    UniqueSeriesSize = p.ReadInt()
                };
                break;
            default: throw new Exception($"Unknown MarketplaceItem type: {itemType}");
        }

        Price = p.ReadInt();
        MinutesRemaining = p.ReadInt();
        Average = p.ReadInt();
        if (hasOfferCount)
            Offers = p.ReadInt();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        if (Data == null)
            throw new Exception("Data cannot be null");

        p.WriteId(Id);
        p.WriteInt((int)Status);

        if (Type == ItemType.Floor)
        {
            if (Data.Flags.HasFlag(ItemDataFlags.IsLimitedRare))
            {
                p.WriteInt(3);
                p.WriteInt(Kind);
                p.WriteInt(Data.UniqueSerialNumber);
                p.WriteInt(Data.UniqueSeriesSize);
            }
            else
            {
                p.WriteInt(1);
                p.WriteInt(Kind);
                p.Compose(Data);
            }
        }
        else if (Type == ItemType.Wall)
        {
            p.WriteInt(2);
            p.WriteInt(Kind);
            p.WriteString(Data.Value);
        }
        else
        {
            throw new Exception($"Invalid MarketplaceItem type: {Type}");
        }

        p.WriteInt(Price);
        p.WriteInt(MinutesRemaining);
        p.WriteInt(Average);

        if (Offers > 0)
            p.WriteInt(Offers);
    }

    public override string ToString() => $"{nameof(MarketplaceOffer)}#{Id}/{Type}:{Kind}";

    static MarketplaceOffer IParser<MarketplaceOffer>.Parse(in PacketReader p) => Parse(in p, true);
    public static MarketplaceOffer Parse(in PacketReader p, bool hasOfferCount) => new(in p, hasOfferCount);
    public static IEnumerable<MarketplaceOffer> ParseAll(in PacketReader p, bool hasOfferCount = true)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        int n = p.ReadLength();
        var offers = new MarketplaceOffer[n];
        for (int i = 0; i < n; i++)
            offers[i] = Parse(in p, hasOfferCount);
        return offers;
    }
}
