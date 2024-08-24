using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class InventoryFragment : IInventoryFragment, ICollection<InventoryItem>, IComposer, IParser<InventoryFragment>
{
    private readonly List<InventoryItem> _list = [];

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

    protected InventoryFragment(in PacketReader p)
    {
        Total = p.Read<int>();
        Index = p.Read<int>();

        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
            _list.Add(p.Parse<InventoryItem>());
    }

    public void Add(InventoryItem item) => _list.Add(item);
    public bool Remove(InventoryItem item) => _list.Remove(item);
    public void Clear() => _list.Clear();
    public bool Contains(InventoryItem item) => _list.Contains(item);
    public void CopyTo(InventoryItem[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    public IEnumerator<InventoryItem> GetEnumerator() => _list.GetEnumerator();
    IEnumerator<IInventoryItem> IEnumerable<IInventoryItem>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Compose(in PacketWriter p)
    {
        p.Write(Total);
        p.Write(Index);

        p.Write<Length>((short)_list.Count);
        foreach (InventoryItem item in _list)
            p.Write(item);
    }

    public static InventoryFragment Parse(in PacketReader packet) => new InventoryFragment(in packet);
}
