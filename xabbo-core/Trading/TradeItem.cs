using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class TradeItem : IInventoryItem
    {
        public static TradeItem Parse(Packet packet) => new TradeItem(packet);

        public int ItemId { get; set; }
        public FurniType Type { get; set; }
        public int Id { get; set; }
        public int Kind { get; set; }
        public FurniCategory Category { get; set; }
        public bool IsGroupable { get; set; }
        public StuffData Data { get; set; }
        public int CreationDay { get; set; }
        public int CreationMonth { get; set; }
        public int CreationYear { get; set; }
        public int Extra { get; set; }

        public TradeItem() { }

        protected TradeItem(Packet packet)
        {
            ItemId = packet.ReadInteger();
            Type = H.ToFurniType(packet.ReadString());
            Id = packet.ReadInteger();
            Kind = packet.ReadInteger();
            Category = (FurniCategory)packet.ReadInteger();
            IsGroupable = packet.ReadBoolean();
            Data = StuffData.Parse(packet);
            CreationDay = packet.ReadInteger();
            CreationMonth = packet.ReadInteger();
            CreationYear = packet.ReadInteger();
            if (Type == FurniType.Floor)
                Extra = packet.ReadInteger();
            else
                Extra = -1;
        }
    }
}
