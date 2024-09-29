using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IActivityPoints"/>
public class ActivityPoints : IActivityPoints, IParserComposer<ActivityPoints>
{
    private readonly Dictionary<ActivityPointType, int> _dict = [];

    IEnumerable<ActivityPointType> IReadOnlyDictionary<ActivityPointType, int>.Keys => _dict.Keys;
    IEnumerable<int> IReadOnlyDictionary<ActivityPointType, int>.Values => _dict.Values;
    public int Count => _dict.Count;

    public int this[ActivityPointType key]
    {
        get
        {
            lock (_dict)
            {
                return _dict[key];
            }
        }

        set
        {
            lock (_dict)
            {
                _dict[key] = value;
            }
        }
    }

    public ActivityPoints() { }

    protected ActivityPoints(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        int n = p.ReadLength();
        for (int i = 0; i < n; i++)
        {
            var type = (ActivityPointType)p.ReadInt();
            _dict[type] = p.ReadInt();
        }
    }

    public bool ContainsKey(ActivityPointType key) => _dict.ContainsKey(key);
    public bool TryGetValue(ActivityPointType key, out int value) => _dict.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<ActivityPointType, int>> GetEnumerator() => _dict.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _dict.GetEnumerator();

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.WriteLength((Length)Count);
        foreach (var (type, amount) in this)
        {
            p.WriteInt((int)type);
            p.WriteInt(amount);
        }
    }

    static ActivityPoints IParser<ActivityPoints>.Parse(in PacketReader p) => new(in p);
}
