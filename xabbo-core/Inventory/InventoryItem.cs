using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class InventoryItem : IItem
    {
        public static InventoryItem Parse(Packet packet) => new InventoryItem(packet);

        // No idea why there are inverse ids, but it
        // looks like it uses InverseId when placing/trading items,
        // and Id when moving/picking up items
        public int InverseId { get; set; }
        public FurniType Type { get; set; }
        public int Id { get; set; }
        public int Kind { get; set; }
        public FurniCategory Category { get; set; }
        public StuffData Data { get; set; }
        public bool Bool1 { get; set; }
        public bool IsTradeable { get; set; }
        public bool IsGroupable { get; set; }
        public bool IsSellable { get; set; } // ?
        public int SecondsToExpiration { get; set; }
        public bool HasRentPeriodStarted { get; set; }
        public int RoomId { get; set; }
        public string String2 { get; set; }
        public int Extra { get; set; }

        public bool IsFloorItem => Type == FurniType.Floor;
        public bool IsWallItem => Type == FurniType.Wall;

        public InventoryItem() { }

        private InventoryItem(Packet packet)
        {
            InverseId = packet.ReadInteger();
            Type = H.ToFurniType(packet.ReadString());
            Id = packet.ReadInteger();
            Kind = packet.ReadInteger();
            Category = (FurniCategory)packet.ReadInteger();
            Data = StuffData.Parse(packet);
            Bool1 = packet.ReadBoolean();
            IsTradeable = packet.ReadBoolean();
            IsGroupable = packet.ReadBoolean();
            IsSellable = packet.ReadBoolean();
            SecondsToExpiration = packet.ReadInteger();
            HasRentPeriodStarted = packet.ReadBoolean();
            RoomId = packet.ReadInteger();

            if (Type == FurniType.Floor)
            {
                String2 = packet.ReadString();
                Extra = packet.ReadInteger(); // same as UnknownInt1 in RoomFloorItem
            }
        }
    }
}
