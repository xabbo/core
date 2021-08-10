using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class InventoryFragment : IInventoryFragment, ICollection<InventoryItem>
    {
        private readonly List<InventoryItem> _list = new List<InventoryItem>();

        public int Total { get; set; }
        public int Index { get; set; }

        bool ICollection<InventoryItem>.IsReadOnly => false;

        public int Count => _list.Count;

        public InventoryItem this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public IEnumerable<InventoryItem> FloorItems => this.Where<InventoryItem>(item => item.IsFloorItem);
        IEnumerable<IInventoryItem> IInventoryFragment.FloorItems => FloorItems;
        public IEnumerable<InventoryItem> WallItems => this.Where<InventoryItem>(item => item.IsWallItem);
        IEnumerable<IInventoryItem> IInventoryFragment.WallItems => WallItems;

        public InventoryFragment() { }

        public InventoryFragment(IEnumerable<IInventoryItem> items)
        {
            _list.AddRange(items.Select(
                item => item is InventoryItem inventoryItem ? inventoryItem : new InventoryItem(item)
            ));
        }

        protected InventoryFragment(IReadOnlyPacket packet)
        {
            Total = packet.ReadInt();
            Index = packet.ReadInt();

            _list.AddRange(InventoryItem.ParseMany(packet));
        }

        public void Add(InventoryItem item) => _list.Add(item);
        public bool Remove(InventoryItem item) => _list.Remove(item);
        public void Clear() => _list.Clear();
        public bool Contains(InventoryItem item) => _list.Contains(item);
        public void CopyTo(InventoryItem[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public IEnumerator<InventoryItem> GetEnumerator() => _list.GetEnumerator();
        IEnumerator<IInventoryItem> IEnumerable<IInventoryItem>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Compose(IPacket packet)
        {
            packet
                .WriteInt(Total)
                .WriteInt(Index);

            packet.WriteLegacyShort((short)_list.Count);
            foreach (InventoryItem item in _list)
            {
                packet.Write(item);
            }
        }

        public static InventoryFragment Parse(IReadOnlyPacket packet) => new InventoryFragment(packet);
    }
}
