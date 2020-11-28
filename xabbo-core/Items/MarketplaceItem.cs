using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class MarketplaceItem : IMarketplaceItem
    {
        public int Id { get; set; }
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
            Id = packet.ReadInt();
            UnknownInt2 = packet.ReadInt();

            int itemType = packet.ReadInt();
            switch (itemType)
            {
                case 1:
                    Type = ItemType.Floor;
                    Kind = packet.ReadInt();
                    Data = ItemData.Parse(packet);
                    break;
                case 2:
                    Type = ItemType.Wall;
                    Kind = packet.ReadInt();
                    Data = new BasicData() { Value = packet.ReadString() };
                    break;
                case 3:
                    Type = ItemType.Floor;
                    Kind = packet.ReadInt();
                    Data = new BasicData()
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

        public void Write(IPacket packet)
        {
            if (Data == null)
                throw new Exception("Data cannot be null");

            packet.WriteInt(Id);
            packet.WriteInt(UnknownInt2);

            if (Type == ItemType.Floor)
            {
                if (Data.Flags.HasFlag(ItemDataFlags.IsLimitedRare))
                {
                    packet.WriteInt(3);
                    packet.WriteInt(Kind);
                    packet.WriteInt(Data.LimitedNumber);
                    packet.WriteInt(Data.LimitedTotal);
                }
                else
                {
                    packet.WriteInt(1);
                    packet.WriteInt(Kind);
                    Data.Write(packet);
                }
            }
            else if (Type == ItemType.Wall)
            {
                packet.WriteInt(2);
                packet.WriteInt(Kind);
                packet.WriteString(Data.Value);
            }
            else
            {
                throw new Exception($"Invlaid MarketplaceItem type: {Type}");
            }
        }

        public static MarketplaceItem Parse(IReadOnlyPacket packet)
          => new MarketplaceItem(packet);
    }
}
