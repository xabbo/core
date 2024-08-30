using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class FloorItems : List<FloorItem>, IParserComposer<FloorItems>
{
    public FloorItems() { }
    public FloorItems(int capacity) : base(capacity) { }

    static FloorItems IParser<FloorItems>.Parse(in PacketReader p)
    {
        int n;
        var ownerDictionary = new Dictionary<long, string>();

        if (p.Client != ClientType.Shockwave)
        {
            n = p.ReadLength();
            for (int i = 0; i < n; i++)
                ownerDictionary.Add(p.ReadId(), p.ReadString());
        }

        n = p.ReadLength();
        var items = new FloorItems(n);
        for (int i = 0; i < n; i++)
        {
            var item = p.Parse<FloorItem>();
            if (ownerDictionary.TryGetValue(item.OwnerId, out string? ownerName))
                item.OwnerName = ownerName;
            items.Add(item);
        }

        return items;
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client != ClientType.Shockwave)
        {
            var ownerIds = new HashSet<long>();
            var ownerDictionary = this
                .Where(x => ownerIds.Add(x.OwnerId))
                .ToDictionary(
                    key => key.OwnerId,
                    val => val.OwnerName
                );

            p.WriteLength(ownerDictionary.Count);
            foreach (var pair in ownerDictionary)
            {
                p.WriteId(pair.Key);
                p.WriteString(pair.Value);
            }
        }

        p.WriteLength(Count);
        foreach (FloorItem item in this)
            p.Compose(item);
    }
}