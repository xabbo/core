using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IStringArrayData"/>
public sealed class StringArrayData : ItemData, IStringArrayData, IList<string>
{
    private readonly List<string> _list;

    public int Count => _list.Count;
    bool ICollection<string>.IsReadOnly => false;

    public string this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    public StringArrayData()
        : base(ItemDataType.StringArray)
    {
        _list = [];
    }

    public StringArrayData(IStringArrayData data)
        : base(data)
    {
        _list = [.. data];
    }

    protected override void Initialize(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        _list.AddRange(p.ReadStringArray());

        base.Initialize(in p);
    }

    protected override void WriteData(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        p.WriteStringArray(_list);
        WriteBase(in p);
    }

    public int IndexOf(string item) => _list.IndexOf(item);
    public void Insert(int index, string item) => _list.Insert(index, item);
    public void RemoveAt(int index) => _list.RemoveAt(index);
    public void Add(string item) => _list.Add(item);
    public void Clear() => _list.Clear();
    public bool Contains(string item) => _list.Contains(item);
    public void CopyTo(string[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public bool Remove(string item) => _list.Remove(item);
    public IEnumerator<string> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
