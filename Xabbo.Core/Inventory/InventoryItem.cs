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
        public StuffData Data { get; set; }
        IItemData IInventoryItem.Data => Data;
        public bool UnknownBool1 { get; set; }
        public bool IsTradeable { get; set; }
        public bool IsGroupable { get; set; }
        public bool IsSellable { get; set; } // ?
        public int SecondsToExpiration { get; set; }
        public bool HasRentPeriodStarted { get; set; }
        public long RoomId { get; set; }
        public string UnknownString1 { get; set; }
        public string UnknownString2 { get; set; }
        public int UnknownInt1 { get; set; }

        public string UnknownString3 { get; set; }
        public int Extra { get; set; }
        public int UnknownInt2 { get; set; }

        public bool IsFloorItem => Type == ItemType.Floor;
        public bool IsWallItem => Type == ItemType.Wall;

        public InventoryItem()
        {
            Data = new LegacyData();

            UnknownString1 =
            UnknownString2 =
            UnknownString3 = string.Empty;
        }

        protected InventoryItem(IReadOnlyPacket packet)
        {
            ItemId = packet.ReadLong();
            Type = H.ToItemType(packet.ReadShort());
            Id = packet.ReadLong();
            Kind = packet.ReadInt();
            Category = (FurniCategory)packet.ReadInt();
            Data = StuffData.Parse(packet);
            UnknownBool1 = packet.ReadBool();
            IsTradeable = packet.ReadBool();
            IsGroupable = packet.ReadBool();
            IsSellable = packet.ReadBool();
            SecondsToExpiration = packet.ReadInt();
            HasRentPeriodStarted = packet.ReadBool();
            RoomId = packet.ReadLong();

            // - Seems to be consistent
            UnknownString1 = packet.ReadString(); // string ""
            UnknownString2 = packet.ReadString(); // string "r" / "s"
            UnknownInt1 = packet.ReadInt(); // int 1187551480

            if (Type == ItemType.Floor)
            {
                // 10 bytes ?
                UnknownString3 = packet.ReadString();
                Extra = packet.ReadInt();
                UnknownInt2 = packet.ReadInt();
            }
            else
            {
                UnknownString3 = string.Empty;
            }
        }

        public void Write(IPacket packet)
        {
            packet.WriteLong(ItemId);
            packet.WriteShort(Type.GetValue());
            packet.WriteLong(Id);
            packet.WriteInt(Kind);
            packet.WriteInt((int)Category);
            Data.Write(packet);
            packet.WriteBool(UnknownBool1);
            packet.WriteBool(IsTradeable);
            packet.WriteBool(IsGroupable);
            packet.WriteBool(IsSellable);
            packet.WriteInt(SecondsToExpiration);
            packet.WriteBool(HasRentPeriodStarted);
            packet.WriteLong(RoomId);

            packet.WriteString(UnknownString1);
            packet.WriteString(UnknownString2);
            packet.WriteInt(UnknownInt1);

            if (Type == ItemType.Floor)
            {
                packet.WriteString(UnknownString3);
                packet.WriteInt(Extra);
                packet.WriteInt(UnknownInt2);
            }
        }
    }
}
