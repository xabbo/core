using System;
using System.Collections.Generic;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public class InventoryItem : IInventoryItem
    {
        public long ItemId { get; set; }
        public ItemType Type { get; set; }
        public long Id { get; set; }
        public int Kind { get; set; }
        public FurniCategory Category { get; set; }
        public ItemData Data { get; set; }
        IItemData IInventoryItem.Data => Data;
        public bool IsRecyclable { get; set; }
        public bool IsTradeable { get; set; }
        public bool IsGroupable { get; set; }
        public bool IsSellable { get; set; }
        public int SecondsToExpiration { get; set; }
        public bool HasRentPeriodStarted { get; set; }
        public long RoomId { get; set; }
        public short _Short1 { get; set; }
        public string SlotId { get; set; }
        public int _Int3 { get; set; }

        public string _String3 { get; set; }
        public long Extra { get; set; }
        public int _Int5 { get; set; }

        public bool IsFloorItem => Type == ItemType.Floor;
        public bool IsWallItem => Type == ItemType.Wall;

        public InventoryItem()
        {
            Data = new LegacyData();

            SlotId =
            _String3 = string.Empty;
        }

        public InventoryItem(IInventoryItem item)
        {
            Type = item.Type;
            Kind = item.Kind;

            Id = item.Id;
            ItemId = item.ItemId;
            Category = item.Category;
            Data = (ItemData)item.Data; // TODO Deep copy data

        }

        protected InventoryItem(IReadOnlyPacket packet)
            : this()
        {
            ItemId = packet.ReadLegacyLong();

            if (packet.Protocol == ClientType.Flash)
            {
                Type = H.ToItemType(packet.ReadString());
            }
            else
            {
                Type = H.ToItemType(packet.ReadShort());
            }

            Id = packet.ReadLegacyLong();
            Kind = packet.ReadInt();
            Category = (FurniCategory)packet.ReadInt();
            Data = ItemData.Parse(packet);
            IsRecyclable = packet.ReadBool();
            IsTradeable = packet.ReadBool();
            IsGroupable = packet.ReadBool();
            IsSellable = packet.ReadBool();
            SecondsToExpiration = packet.ReadInt();
            HasRentPeriodStarted = packet.ReadBool();
            RoomId = packet.ReadLegacyLong();

            if (packet.Protocol == ClientType.Unity)
            {
                // - Seems to be consistent
                _Short1 = packet.ReadShort(); // ?
                SlotId = packet.ReadString(); // string "r" / "s"
                _Int3 = packet.ReadInt(); // int 1187551480
            }

            if (Type == ItemType.Floor)
            {
                if (packet.Protocol == ClientType.Flash)
                {
                    SlotId = packet.ReadString();
                    Extra = packet.ReadInt();
                }
                else
                {
                    // 10 bytes ?
                    _String3 = packet.ReadString();
                    Extra = packet.ReadInt();
                    _Int5 = packet.ReadInt();
                }
            }
            else
            {
                _String3 = string.Empty;
            }

            /*if (clientType == ClientType.Flash)
            {
                ItemId = packet.ReadInt();
                Type = H.ToItemType(packet.ReadString());
                Id = packet.ReadInt();
                Kind = packet.ReadInt();
                Category = (FurniCategory)packet.ReadInt();
                Data = StuffData.Parse(packet, clientType);
                _Bool1 = packet.ReadBool();
                IsTradeable = packet.ReadBool();
                IsGroupable = packet.ReadBool();
                IsSellable = packet.ReadBool();
                SecondsToExpiration = packet.ReadInt();
                HasRentPeriodStarted = packet.ReadBool();
                RoomId = packet.ReadInt();

                if (Type == ItemType.Floor)
                {
                    _String2 = packet.ReadString();
                    Extra = packet.ReadInt();
                }
            }
            else
            {
                ItemId = packet.ReadLong();
                Type = H.ToItemType(packet.ReadShort());
                Id = packet.ReadLong();
                Kind = packet.ReadInt();
                Category = (FurniCategory)packet.ReadInt();
                Data = StuffData.Parse(packet, clientType);
                _Bool1 = packet.ReadBool();
                IsTradeable = packet.ReadBool();
                IsGroupable = packet.ReadBool();
                IsSellable = packet.ReadBool();
                SecondsToExpiration = packet.ReadInt();
                HasRentPeriodStarted = packet.ReadBool();
                RoomId = packet.ReadLong();

                // - Seems to be consistent
                _String1 = packet.ReadString(); // string ""
                _String2 = packet.ReadString(); // string "r" / "s"
                _Int3 = packet.ReadInt(); // int 1187551480

                if (Type == ItemType.Floor)
                {
                    // 10 bytes ?
                    _String3 = packet.ReadString();
                    Extra = packet.ReadInt();
                    _Int5 = packet.ReadInt();
                }
                else
                {
                    _String3 = string.Empty;
                }
            }*/
        }

        public void Compose(IPacket packet)
        {
            packet.WriteLegacyLong(ItemId);

            if (packet.Protocol == ClientType.Flash)
            {
                packet.WriteString(Type.ToShortString().ToUpper());
            }
            else
            {
                packet.WriteShort(Type.GetValue());
            }

            packet
                .WriteLegacyLong(Id)
                .WriteInt(Kind)
                .WriteInt((int)Category)
                .Write(Data)
                .WriteBool(IsRecyclable)
                .WriteBool(IsTradeable)
                .WriteBool(IsGroupable)
                .WriteBool(IsSellable)
                .WriteInt(SecondsToExpiration)
                .WriteBool(HasRentPeriodStarted)
                .WriteLegacyLong(RoomId);

            if (packet.Protocol == ClientType.Unity)
            {
                packet
                    .WriteShort(_Short1)
                    .WriteString(SlotId)
                    .WriteInt(_Int3);
            }

            if (Type == ItemType.Floor)
            {
                if (packet.Protocol == ClientType.Flash)
                {
                    packet
                        .WriteString(SlotId)
                        .WriteLegacyLong(Extra);
                }
                else
                {
                    // 10 bytes ?
                    packet
                        .WriteString(_String3)
                        .WriteLegacyLong(Extra)
                        .WriteInt(_Int5);
                }
            }
        }

        public static InventoryItem Parse(IReadOnlyPacket packet) => new InventoryItem(packet);

        public static IEnumerable<InventoryItem> ParseMany(IReadOnlyPacket packet)
        {
            short n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
            {
                yield return Parse(packet);
            }
        }

        public override string ToString() => $"[{Id}:{Type.ToShortString()}{Kind}]";
    }
}
