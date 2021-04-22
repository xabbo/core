using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class Inventory : IInventory, ICollection<InventoryItem>
    {
        private readonly List<InventoryItem> _list = new List<InventoryItem>();

        public int TotalPackets { get; set; }
        public int PacketIndex { get; set; }

        bool ICollection<InventoryItem>.IsReadOnly => false;

        public int Count => _list.Count;

        public InventoryItem this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
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

            foreach (InventoryItem item in ParseItems(packet))
            {
                Add(item);
            }
        }

        public void Add(InventoryItem item) => _list.Add(item);
        public bool Remove(InventoryItem item) => _list.Remove(item);
        public void Clear() => _list.Clear();
        public bool Contains(InventoryItem item) => _list.Contains(item);
        public void CopyTo(InventoryItem[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public IEnumerator<InventoryItem> GetEnumerator() => _list.GetEnumerator();
        IEnumerator<IInventoryItem> IEnumerable<IInventoryItem>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static Inventory Parse(IReadOnlyPacket packet)
        {
            return new Inventory(packet);
        }

        public static IEnumerable<InventoryItem> ParseItems(IReadOnlyPacket packet)
        {
            int n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
            {
                yield return InventoryItem.Parse(packet);
            }
        }
    }
}
