using System.Collections;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class ActivityPoints : IActivityPoints, IComposer, IParser<ActivityPoints>
{
    public static ActivityPoints Parse(in PacketReader packet) => new(in packet);

    private readonly Dictionary<ActivityPointType, int> dictionary = [];

    public IEnumerable<ActivityPointType> Keys => dictionary.Keys;
    public IEnumerable<int> Values => dictionary.Values;
    public int Count => dictionary.Count;

    public int this[ActivityPointType key]
    {
        get
        {
            lock (dictionary)
            {
                return dictionary[key];
            }
        }

        set
        {
            lock (dictionary)
            {
                dictionary[key] = value;
            }
        }
    }

    public ActivityPoints() { }

    protected ActivityPoints(in PacketReader p)
    {
        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
        {
            var type = (ActivityPointType)p.Read<int>();
            dictionary[type] = p.Read<int>();
        }
    }

    public bool ContainsKey(ActivityPointType key) => dictionary.ContainsKey(key);

    public IEnumerator<KeyValuePair<ActivityPointType, int>> GetEnumerator() => dictionary.GetEnumerator();
    public bool TryGetValue(ActivityPointType key, out int value) => dictionary.TryGetValue(key, out value);
    IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();

    public void Compose(in PacketWriter p)
    {
        p.Write<Length>(Count);
        foreach (var (type, amount) in this)
        {
            p.Write((int)type);
            p.Write(amount);
        }
    }
}
