using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IMapData"/>
public sealed class MapData : ItemData, IMapData, IDictionary<string, string>
{
    private readonly Dictionary<string, string> _map;

    public ICollection<string> Keys => _map.Keys;
    IEnumerable<string> IReadOnlyDictionary<string, string>.Keys => Keys;
    public ICollection<string> Values => _map.Values;
    IEnumerable<string> IReadOnlyDictionary<string, string>.Values => Values;
    public int Count => _map.Count;

    bool ICollection<KeyValuePair<string, string>>.IsReadOnly => false;

    public string this[string key]
    {
        get => _map[key];
        set => _map[key] = value;
    }

    public MapData() : base(ItemDataType.Map)
    {
        _map = [];
    }

    public MapData(IMapData data) : base(data)
    {
        _map = new Dictionary<string, string>(data);
    }

    protected override void Initialize(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        int n = p.ReadLength();
        for (int i = 0; i < n; i++)
            _map.Add(p.ReadString(), p.ReadString());

        base.Initialize(in p);
    }

    protected override void WriteData(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        p.WriteLength((Length)_map.Count);
        foreach (var item in _map)
        {
            p.WriteString(item.Key);
            p.WriteString(item.Value);
        }

        WriteBase(in p);
    }

    public bool ContainsKey(string key) => _map.ContainsKey(key);
    public void Add(string key, string value) => _map.Add(key, value);
    public bool Remove(string key) => _map.Remove(key);
    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value) => _map.TryGetValue(key, out value);
    public void Add(KeyValuePair<string, string> item) => ((IDictionary<string, string>)_map).Add(item);
    public void Clear() => _map.Clear();
    public bool Contains(KeyValuePair<string, string> item) => ((IDictionary<string, string>)_map).Contains(item);
    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) => ((IDictionary<string, string>)_map).CopyTo(array, arrayIndex);
    public bool Remove(KeyValuePair<string, string> item) => ((IDictionary<string, string>)_map).Remove(item);
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _map.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
