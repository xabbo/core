using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class MarketplaceItem : IMarketplaceItem
    {
        public long Id { get; set; }
        public int UnknownInt2 { get; set; }
        public ItemType Type { get; set; }
        public int Kind { get; set; }
        public IItemData Data { get; set; }
        public int LimitedNumber { get; set; }
        public int LimitedTotal { get; set; }
        public int Price { get; set; }
        public int UnknownInt7 { get; set; }
        public int Average { get; set; }
        public int Offers { get; set; }

        protected MarketplaceItem(IReadOnlyPacket packet)
        {
            Id = packet.ReadLegacyLong();
            UnknownInt2 = packet.ReadInt();

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
            UnknownInt7 = packet.ReadInt();
            Average = packet.ReadInt();
            Offers = packet.ReadInt();
        }

        public void Compose(IPacket packet)
        {
            if (Data == null)
                throw new Exception("Data cannot be null");

            packet
                .WriteLegacyLong(Id)
                .WriteInt(UnknownInt2);

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
        }

        public static MarketplaceItem Parse(IReadOnlyPacket packet)
        { 
            return new MarketplaceItem(packet);
        }
    }
}
