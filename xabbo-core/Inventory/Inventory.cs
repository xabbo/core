using System;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Inventory : List<InventoryItem>
    {
        public static Inventory Parse(Packet packet) => new Inventory(packet);

        public static IEnumerable<InventoryItem> ParseItems(Packet packet)
        {
            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                yield return InventoryItem.Parse(packet);
        }

        internal int TotalPackets { get; set; }
        internal int PacketIndex { get; set; }

        public Inventory() { }

        internal Inventory(Packet packet)
        {
            TotalPackets = packet.ReadInteger();
            PacketIndex = packet.ReadInteger();

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                Add(InventoryItem.Parse(packet));
        }

        public IEnumerable<InventoryItem> FloorItems => this.Where(item => item.IsFloorItem);
        public IEnumerable<InventoryItem> WallItems => this.Where(item => item.IsWallItem);
    }
}
