using System;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class WallItems : List<WallItem>, IParserComposer<WallItems>
{
    public WallItems() { }
    public WallItems(int capacity) : base(capacity) { }
    public WallItems(IEnumerable<WallItem> items) : base(items) { }

    static WallItems IParser<WallItems>.Parse(in PacketReader p)
    {
        if (p.Client == ClientType.Shockwave)
        {
            var items = new WallItems();
            while (p.Available > 0)
                items.Add(p.Parse<WallItem>());
            return items;
        }
        else
        {
            var ownerDictionary = new Dictionary<long, string>();

            int n = p.ReadLength();
            for (int i = 0; i < n; i++)
                ownerDictionary.Add(p.ReadId(), p.ReadString());

            n = p.ReadLength();
            var items = new WallItems(n);
            for (int i = 0; i < n; i++)
            {
                var item = p.Parse<WallItem>();
                if (ownerDictionary.TryGetValue(item.OwnerId, out string? ownerName))
                    item.OwnerName = ownerName;
                items.Add(item);
            }

            return items;
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client == ClientType.Shockwave)
            throw new NotImplementedException("WallItem.ComposeAll is not implemented for Shockwave");

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

        p.WriteLength(Count);
        foreach (var item in this)
            p.Compose(item);
    }
}