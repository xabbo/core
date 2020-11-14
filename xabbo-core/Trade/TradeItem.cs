using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class TradeItem : ITradeItem
    {
        public int ItemId { get; set; }
        public ItemType Type { get; set; }
        public bool IsFloorItem => Type == ItemType.Floor;
        public bool IsWallItem => Type == ItemType.Wall;
        public int Id { get; set; }
        public int Kind { get; set; }
        public FurniCategory Category { get; set; }
        public bool IsGroupable { get; set; }
        public ItemData Data { get; set; }
        IItemData IInventoryItem.Data => Data;
        public int CreationDay { get; set; }
        public int CreationMonth { get; set; }
        public int CreationYear { get; set; }
        public int Extra { get; set; }

        bool IInventoryItem.IsTradeable => true;
        bool IInventoryItem.IsSellable => true;
        int IInventoryItem.SecondsToExpiration => -1;
        bool IInventoryItem.HasRentPeriodStarted => false;
        int IInventoryItem.RoomId => -1;

        public TradeItem() { }

        protected TradeItem(IReadOnlyPacket packet)
        {
            ItemId = packet.ReadInt();
            Type = H.ToItemType(packet.ReadString());
            Id = packet.ReadInt();
            Kind = packet.ReadInt();
            Category = (FurniCategory)packet.ReadInt();
            IsGroupable = packet.ReadBool();
            Data = ItemData.Parse(packet);
            CreationDay = packet.ReadInt();
            CreationMonth = packet.ReadInt();
            CreationYear = packet.ReadInt();

            if (Type == ItemType.Floor)
                Extra = packet.ReadInt();
            else
                Extra = -1;
        }

        public static TradeItem Parse(IReadOnlyPacket packet) => new TradeItem(packet);

        public void Write(IPacket packet)
        {
            packet.WriteInt(ItemId);
            packet.WriteString(Type.ToShortString());
            packet.WriteInt(Id);
            packet.WriteInt(Kind);
            packet.WriteInt((int)Category);
            packet.WriteBool(IsGroupable);
            Data.Write(packet);
            packet.WriteInt(CreationDay);
            packet.WriteInt(CreationMonth);
            packet.WriteInt(CreationYear);

            if (Type == ItemType.Floor)
                packet.WriteInt(Extra);

        }
    }
}
