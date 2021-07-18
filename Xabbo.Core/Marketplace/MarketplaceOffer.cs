using System;
using System.Collections;
using System.Collections.Generic;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public class MarketplaceOffer : IMarketplaceOffer
    {
        public long Id { get; set; }
        public MarketplaceOfferStatus Status { get; set; }
        public ItemType Type { get; set; }
        public int Kind { get; set; }
        public IItemData Data { get; set; }
        public int LimitedNumber { get; set; }
        public int LimitedTotal { get; set; }
        public int Price { get; set; }
        public int TimeRemaining { get; set; }
        public int Average { get; set; }
        public int Offers { get; set; }

        public MarketplaceOffer()
        {
            Data = new LegacyData();
        }

        protected MarketplaceOffer(IReadOnlyPacket packet, bool hasOfferCount)
        {
            Id = packet.ReadLegacyLong();
            Status = (MarketplaceOfferStatus)packet.ReadInt();

            int itemType = packet.ReadInt();
            switch (itemType)
            {
                case 1:
                    Type = ItemType.Floor;
                    Kind = packet.ReadInt();
                    Data = StuffData.Parse(packet);
                    break;
                case 2:
                    Type = ItemType.Wall;
                    Kind = packet.ReadInt();
                    Data = new LegacyData() { Value = packet.ReadString() };
                    break;
                case 3:
                    Type = ItemType.Floor;
                    Kind = packet.ReadInt();
                    Data = new LegacyData()
                    {
                        Flags = ItemDataFlags.IsLimitedRare,
                        LimitedNumber = packet.ReadInt(),
                        LimitedTotal = packet.ReadInt()
                    };
                    break;
                default: throw new Exception($"Unknown MarketplaceItem type: {itemType}");
            }

            Price = packet.ReadInt();
            TimeRemaining = packet.ReadInt();
            Average = packet.ReadInt();
            if (hasOfferCount)
                Offers = packet.ReadInt();
        }

        public void Compose(IPacket packet)
        {
            if (Data == null)
                throw new Exception("Data cannot be null");

            packet
                .WriteLegacyLong(Id)
                .WriteInt((int)Status);

            if (Type == ItemType.Floor)
            {
                if (Data.Flags.HasFlag(ItemDataFlags.IsLimitedRare))
                {
                    packet
                        .WriteInt(3)
                        .WriteInt(Kind)
                        .WriteInt(Data.LimitedNumber)
                        .WriteInt(Data.LimitedTotal);
                }
                else
                {
                    packet
                        .WriteInt(1)
                        .WriteInt(Kind)
                        .Write(Data);
                }
            }
            else if (Type == ItemType.Wall)
            {
                packet
                    .WriteInt(2)
                    .WriteInt(Kind)
                    .WriteString(Data.Value);
            }
            else
            {
                throw new Exception($"Invalid MarketplaceItem type: {Type}");
            }

            packet
                .WriteInt(Price)
                .WriteInt(TimeRemaining)
                .WriteInt(Average);

            if (Offers > 0)
                packet.WriteInt(Offers);
        }

        public static MarketplaceOffer Parse(IReadOnlyPacket packet, bool hasOfferCount = true)
        { 
            return new MarketplaceOffer(packet, hasOfferCount);
        }

        public static IEnumerable<MarketplaceOffer> ParseMany(IReadOnlyPacket packet, bool hasOfferCount = true)
        {
            short n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
                yield return Parse(packet, hasOfferCount);
        }

        public override string ToString() => $"{Id}/{Type.ToShortString()}:{Kind}";
    }
}
