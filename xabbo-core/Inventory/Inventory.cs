using System;
using System.Linq;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Inventory : List<InventoryItem>, IInventory
    {
        public static Inventory Parse(Packet packet) => new Inventory(packet);

        public static IEnumerable<InventoryItem> ParseItems(Packet packet)
        {
            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                yield return InventoryItem.Parse(packet);
        }

        internal int TotalPackets { get; set; }
        internal int PacketIndex { get; set; }

        IInventoryItem IReadOnlyList<IInventoryItem>.this[int index] => this[index];
        IEnumerator<IInventoryItem> IEnumerable<IInventoryItem>.GetEnumerator() => GetEnumerator();

        public Inventory() { }

        internal Inventory(Packet packet)
        {
            TotalPackets = packet.ReadInt();
            PacketIndex = packet.ReadInt();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                Add(InventoryItem.Parse(packet));
        }

        public IEnumerable<InventoryItem> FloorItems => ((IEnumerable<InventoryItem>)this).Where(item => item.IsFloorItem);
        IEnumerable<IInventoryItem> IInventory.FloorItems => FloorItems;
        public IEnumerable<InventoryItem> WallItems => ((IEnumerable<InventoryItem>)this).Where(item => item.IsWallItem);
        IEnumerable<IInventoryItem> IInventory.WallItems => WallItems;
    }
}
