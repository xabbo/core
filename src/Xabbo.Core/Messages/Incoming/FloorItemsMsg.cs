using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed class FloorItemsMsg : List<FloorItem>, IMessage<FloorItemsMsg>
{
    public FloorItemsMsg() { }
    public FloorItemsMsg(int capacity) : base(capacity) { }

    public static Identifier Identifier => In.Objects;

    static FloorItemsMsg IParser<FloorItemsMsg>.Parse(in PacketReader p)
    {
        int n;
        var ownerDictionary = new Dictionary<long, string>();

        if (p.Client is not ClientType.Shockwave)
        {
            n = p.ReadLength();
            for (int i = 0; i < n; i++)
                ownerDictionary.Add(p.ReadId(), p.ReadString());
        }

        n = p.ReadLength();
        var items = new FloorItemsMsg(n);
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
        if (p.Client is not ClientType.Shockwave)
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