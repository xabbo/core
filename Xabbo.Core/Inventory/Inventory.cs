using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class Inventory : IInventory, ICollection<InventoryItem>
    {
        public static Inventory Parse(IReadOnlyPacket packet) => new Inventory(packet);

        public static IEnumerable<InventoryItem> ParseItems(IReadOnlyPacket packet)
        {
            short n = packet.ReadShort();
            for (int i = 0; i < n; i++)
                yield return InventoryItem.Parse(packet);
        }

        private readonly List<InventoryItem> list = new List<InventoryItem>();

        public int TotalPackets { get; set; }
        public int PacketIndex { get; set; }

        bool ICollection<InventoryItem>.IsReadOnly => false;

        public int Count => list.Count;

        public InventoryItem this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public IEnumerable<InventoryItem> FloorItems => this.Where<InventoryItem>(item => item.IsFloorItem);
        IEnumerable<IInventoryItem> IInventory.FloorItems => FloorItems;
        public IEnumerable<InventoryItem> WallItems => this.Where<InventoryItem>(item => item.IsWallItem);
        IEnumerable<IInventoryItem> IInventory.WallItems => WallItems;

        public Inventory() { }

        protected Inventory(IReadOnlyPacket packet)
        {
            TotalPackets = packet.ReadInt();
            PacketIndex = packet.ReadInt();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                Add(InventoryItem.Parse(packet));
        }

        public void Add(InventoryItem item) => list.Add(item);
        public bool Remove(InventoryItem item) => list.Remove(item);
        public void Clear() => list.Clear();
        public bool Contains(InventoryItem item) => list.Contains(item);
        public void CopyTo(InventoryItem[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        public IEnumerator<InventoryItem> GetEnumerator() => list.GetEnumerator();
        IEnumerator<IInventoryItem> IEnumerable<IInventoryItem>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
