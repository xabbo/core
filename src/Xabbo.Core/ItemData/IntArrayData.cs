using System;
using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class IntArrayData : ItemData, IIntArrayData, IList<int>
{
    private readonly List<int> _list;

    public int Count => _list.Count;
    bool ICollection<int>.IsReadOnly => false;

    public int this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    public IntArrayData()
        : base(ItemDataType.IntArray)
    {
        _list = new List<int>();
    }

    public IntArrayData(IIntArrayData data)
        : base(data)
    {
        _list = new List<int>(data);
    }

    protected override void Initialize(IReadOnlyPacket packet)
    {
        short n = packet.ReadLegacyShort();
        for (int i = 0; i < n; i++)
        {
            _list.Add(packet.ReadInt());
        }

        base.Initialize(packet);
    }

    protected override void WriteData(IPacket packet)
    {
        packet.WriteLegacyShort((short)Count);
        foreach (int value in this)
        {
            packet.WriteInt(value);
        }

        WriteBase(packet);
    }

    public int IndexOf(int item) => _list.IndexOf(item);
    public void Insert(int index, int item) => _list.Insert(index, item);
    public void RemoveAt(int index) => _list.RemoveAt(index);
    public void Add(int item) => _list.Add(item);
    public void Clear() => _list.Clear();
    public bool Contains(int item) => _list.Contains(item);
    public void CopyTo(int[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public bool Remove(int item) => _list.Remove(item);
    public IEnumerator<int> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
}
