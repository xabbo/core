using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class InventoryItem : IInventoryItem
    {
        public static InventoryItem Parse(IReadOnlyPacket packet) => new InventoryItem(packet);

        public long ItemId { get; set; }
        public ItemType Type { get; set; }
        public long Id { get; set; }
        public int Kind { get; set; }
        public FurniCategory Category { get; set; }
        public ItemData Data { get; set; }
        IItemData IInventoryItem.Data => Data;
        public bool Bool1 { get; set; }
        public bool IsTradeable { get; set; }
        public bool IsGroupable { get; set; }
        public bool IsSellable { get; set; } // ?
        public int SecondsToExpiration { get; set; }
        public bool HasRentPeriodStarted { get; set; }
        public long RoomId { get; set; }
        public string String2 { get; set; }
        public int Extra { get; set; }

        public bool IsFloorItem => Type == ItemType.Floor;
        public bool IsWallItem => Type == ItemType.Wall;

        public InventoryItem() { }

        protected InventoryItem(IReadOnlyPacket packet)
        {
            ItemId = packet.ReadInt();
            Type = H.ToItemType(packet.ReadString());
            Id = packet.ReadInt();
            Kind = packet.ReadInt();
            Category = (FurniCategory)packet.ReadInt();
            Data = ItemData.Parse(packet);
            Bool1 = packet.ReadBool();
            IsTradeable = packet.ReadBool();
            IsGroupable = packet.ReadBool();
            IsSellable = packet.ReadBool();
            SecondsToExpiration = packet.ReadInt();
            HasRentPeriodStarted = packet.ReadBool();
            RoomId = packet.ReadInt();

            if (Type == ItemType.Floor)
            {
                String2 = packet.ReadString();
                Extra = packet.ReadInt();
            }
        }

        public void Write(IPacket packet)
        {
            packet.WriteLong(ItemId);
            packet.WriteString(Type.ToShortString());
            packet.WriteLong(Id);
            packet.WriteInt(Kind);
            packet.WriteInt((int)Category);
            Data.Write(packet);
            packet.WriteBool(Bool1);
            packet.WriteBool(IsTradeable);
            packet.WriteBool(IsGroupable);
            packet.WriteBool(IsSellable);
            packet.WriteInt(SecondsToExpiration);
            packet.WriteBool(HasRentPeriodStarted);
            packet.WriteLong(RoomId);

            if (Type == ItemType.Floor)
            {
                packet.WriteString(String2);
                packet.WriteInt(Extra);
            }
        }
    }
}
