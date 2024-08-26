using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class MarketplaceOffer : IMarketplaceOffer, IComposer, IParser<MarketplaceOffer>
{
    public Id Id => -1;
    public Id OfferId { get; set; }
    public MarketplaceOfferStatus Status { get; set; }
    public ItemType Type { get; set; }
    public int Kind { get; set; }
    public IItemData Data { get; set; }
    public int Price { get; set; }
    public int TimeRemaining { get; set; }
    public int Average { get; set; }
    public int Offers { get; set; }

    public MarketplaceOffer()
    {
        Data = new LegacyData();
    }

    private MarketplaceOffer(in PacketReader p, bool hasOfferCount)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        OfferId = p.Read<Id>();
        Status = (MarketplaceOfferStatus)p.Read<int>();

        int itemType = p.Read<int>();
        switch (itemType)
        {
            case 1:
                Type = ItemType.Floor;
                Kind = p.Read<int>();
                Data = ItemData.Parse(p);
                break;
            case 2:
                Type = ItemType.Wall;
                Kind = p.Read<int>();
                Data = new LegacyData() { Value = p.Read<string>() };
                break;
            case 3:
                Type = ItemType.Floor;
                Kind = p.Read<int>();
                Data = new LegacyData()
                {
                    Flags = ItemDataFlags.IsLimitedRare,
                    UniqueSerialNumber = p.Read<int>(),
                    UniqueSeriesSize = p.Read<int>()
                };
                break;
            default: throw new Exception($"Unknown MarketplaceItem type: {itemType}");
        }

        Price = p.Read<int>();
        TimeRemaining = p.Read<int>();
        Average = p.Read<int>();
        if (hasOfferCount)
            Offers = p.Read<int>();
    }

    public void Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        if (Data == null)
            throw new Exception("Data cannot be null");

        p.Write(Id);
        p.Write((int)Status);

        if (Type == ItemType.Floor)
        {
            if (Data.Flags.HasFlag(ItemDataFlags.IsLimitedRare))
            {
                p.Write(3);
                p.Write(Kind);
                p.Write(Data.UniqueSerialNumber);
                p.Write(Data.UniqueSeriesSize);
            }
            else
            {
                p.Write(1);
                p.Write(Kind);
                p.Write(Data);
            }
        }
        else if (Type == ItemType.Wall)
        {
            p.Write(2);
            p.Write(Kind);
            p.Write(Data.Value);
        }
        else
        {
            throw new Exception($"Invalid MarketplaceItem type: {Type}");
        }

        p.Write(Price);
        p.Write(TimeRemaining);
        p.Write(Average);

        if (Offers > 0)
            p.Write(Offers);
    }

    public override string ToString() => $"{nameof(MarketplaceOffer)}#{Id}/{Type}:{Kind}";

    public static MarketplaceOffer Parse(in PacketReader p) => Parse(in p, true);
    public static MarketplaceOffer Parse(in PacketReader p, bool hasOfferCount) => new(in p, hasOfferCount);
    public static IEnumerable<MarketplaceOffer> ParseAll(in PacketReader p, bool hasOfferCount = true)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        int n = p.Read<Length>();
        var offers = new MarketplaceOffer[n];
        for (int i = 0; i < n; i++)
            offers[i] = Parse(in p, hasOfferCount);
        return offers;
    }
}
