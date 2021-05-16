using System;

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
        public StuffData Data { get; set; }
        IItemData IInventoryItem.Data => Data;
        public bool _Bool1 { get; set; }
        public bool IsTradeable { get; set; }
        public bool IsGroupable { get; set; }
        public bool IsSellable { get; set; } // ?
        public int SecondsToExpiration { get; set; }
        public bool HasRentPeriodStarted { get; set; }
        public long RoomId { get; set; }
        public string _String1 { get; set; }
        public string _String2 { get; set; }
        public int _Int3 { get; set; }

        public string _String3 { get; set; }
        public long Extra { get; set; }
        public int _Int5 { get; set; }

        public bool IsFloorItem => Type == ItemType.Floor;
        public bool IsWallItem => Type == ItemType.Wall;

        public InventoryItem()
        {
            Data = new LegacyData();

            _String1 =
            _String2 =
            _String3 = string.Empty;
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
            Data = StuffData.Parse(packet);
            _Bool1 = packet.ReadBool();
            IsTradeable = packet.ReadBool();
            IsGroupable = packet.ReadBool();
            IsSellable = packet.ReadBool();
            SecondsToExpiration = packet.ReadInt();
            HasRentPeriodStarted = packet.ReadBool();
            RoomId = packet.ReadLegacyLong();

            if (packet.Protocol == ClientType.Unity)
            {
                // - Seems to be consistent
                _String1 = packet.ReadString(); // string ""
                _String2 = packet.ReadString(); // string "r" / "s"
                _Int3 = packet.ReadInt(); // int 1187551480
            }

            if (Type == ItemType.Floor)
            {
                if (packet.Protocol == ClientType.Flash)
                {
                    _String2 = packet.ReadString();
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
            if (packet.Protocol == ClientType.Flash)
            {
                packet
                    .WriteInt((int)ItemId)
                    .WriteString(Type.ToShortString())
                    .WriteInt((int)Id)
                    .WriteInt(Kind)
                    .WriteInt((int)Category)
                    .Write(Data)
                    .WriteBool(_Bool1)
                    .WriteBool(IsTradeable)
                    .WriteBool(IsGroupable)
                    .WriteBool(IsSellable)
                    .WriteInt(SecondsToExpiration)
                    .WriteBool(HasRentPeriodStarted)
                    .WriteInt((int)RoomId);

                if (Type == ItemType.Floor)
                {
                    packet
                        .WriteString(_String2)
                        .WriteInt((int)Extra);
                }
            }
            else if (packet.Protocol == ClientType.Unity)
            {
                packet
                    .WriteLong(ItemId)
                    .WriteShort(Type.GetValue())
                    .WriteLong(Id)
                    .WriteInt(Kind)
                    .WriteInt((int)Category)
                    .Write(Data)
                    .WriteBool(_Bool1)
                    .WriteBool(IsTradeable)
                    .WriteBool(IsGroupable)
                    .WriteBool(IsSellable)
                    .WriteInt(SecondsToExpiration)
                    .WriteBool(HasRentPeriodStarted)
                    .WriteLong(RoomId);
                
                packet
                    .WriteString(_String1)
                    .WriteString(_String2)
                    .WriteInt(_Int3);

                if (Type == ItemType.Floor)
                {
                    packet
                        .WriteString(_String3)
                        .WriteLong(Extra)
                        .WriteInt(_Int5);
                }
            }
            else
            {
                throw new InvalidOperationException("Unknown protocol.");
            }
        }

        public static InventoryItem Parse(IReadOnlyPacket packet)
        {
            return new InventoryItem(packet);
        }

        public override string ToString() => $"[{Id}:{Type.ToShortString()}{Kind}]";
    }
}
