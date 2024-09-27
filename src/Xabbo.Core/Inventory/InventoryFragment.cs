using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IInventoryFragment"/>
public sealed class InventoryFragment : IInventoryFragment, ICollection<InventoryItem>, IParserComposer<InventoryFragment>
{
    private readonly List<InventoryItem> _list;

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

    public InventoryFragment()
    {
        _list = [];
    }

    public InventoryFragment(IEnumerable<IInventoryItem> items)
    {
        _list = [.. items.Select(it => it is InventoryItem item ? item : new InventoryItem(it))];
    }

    private InventoryFragment(in PacketReader p)
    {
        Total = p.ReadInt();
        Index = p.ReadInt();
        _list = [.. p.ParseArray<InventoryItem>()];
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Total);
        p.WriteInt(Index);
        p.ComposeArray(_list);
    }

    public void Add(InventoryItem item) => _list.Add(item);
    public bool Remove(InventoryItem item) => _list.Remove(item);
    public void Clear() => _list.Clear();
    public bool Contains(InventoryItem item) => _list.Contains(item);
    public void CopyTo(InventoryItem[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

    public IEnumerator<InventoryItem> GetEnumerator() => _list.GetEnumerator();
    IEnumerator<IInventoryItem> IEnumerable<IInventoryItem>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    static InventoryFragment IParser<InventoryFragment>.Parse(in PacketReader p) => new(in p);
}
