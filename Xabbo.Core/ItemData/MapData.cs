using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Xabbo.Messages;

namespace Xabbo.Core;

public class MapData : ItemData, IMapData, IDictionary<string, string>
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

    public MapData()
        : base(ItemDataType.Map)
    {
        _map = new Dictionary<string, string>();
    }

    public MapData(IMapData data)
        : base(data)
    {
        _map = new Dictionary<string, string>(data);
    }

    protected override void Initialize(IReadOnlyPacket packet)
    {
        short n = packet.ReadLegacyShort();
        for (int i = 0; i < n; i++)
        {
            _map.Add(packet.ReadString(), packet.ReadString());
        }

        base.Initialize(packet);
    }

    protected override void WriteData(IPacket packet)
    {
        packet.WriteLegacyShort((short)_map.Count);
        foreach (var item in _map)
        {
            packet.WriteString(item.Key);
            packet.WriteString(item.Value);
        }

        WriteBase(packet);
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
