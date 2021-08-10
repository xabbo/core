using System;
using System.Linq;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Xabbo.Core.Game
{
    public class Inventory : IInventory, IEnumerable<InventoryItem>
    {
        private readonly ConcurrentDictionary<long, InventoryItem> _items = new();

        public bool IsInvalidated { get; set; }

        public Inventory() { }

        public Inventory(IEnumerable<InventoryItem> items)
        {
            foreach (InventoryItem item in items)
            {
                TryAdd(item);
            }
        }

        public bool TryAdd(InventoryItem item) => _items.TryAdd(item.ItemId, item);

        public InventoryItem AddOrUpdate(InventoryItem item, out bool added)
        {
            return _items.AddOrUpdate(item.ItemId, item, (id, existingItem) => item, out added);
        }

        public InventoryItem? GetItem(long id) => TryGetItem(id, out InventoryItem? item) ? item : null;
        IInventoryItem? IInventory.GetItem(long id) => GetItem(id);
        public bool TryGetItem(long itemId, [NotNullWhen(true)] out InventoryItem? item) => _items.TryGetValue(itemId, out item);
        bool IInventory.TryGetItem(long id, [NotNullWhen(true)] out IInventoryItem? item) => (item = GetItem(id)) is not null;

        public bool TryRemove(long itemId, [NotNullWhen(true)] out InventoryItem? item) => _items.TryRemove(itemId, out item);
        public void Clear() => _items.Clear();

        public IEnumerator<InventoryItem> GetEnumerator() => _items.Select(x => x.Value).GetEnumerator();
        IEnumerator<IInventoryItem> IEnumerable<IInventoryItem>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
