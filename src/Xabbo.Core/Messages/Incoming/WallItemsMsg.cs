using System.Collections.Generic;
using System.Linq;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Represents a list of wall items.
/// <para/>
/// Received when wall items in the room are loaded.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.Items"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.ITEMS"/></item>
/// </list>
/// </summary>
public sealed class WallItemsMsg : List<WallItem>, IMessage<WallItemsMsg>
{
    public WallItemsMsg() { }
    public WallItemsMsg(int capacity) : base(capacity) { }
    public WallItemsMsg(IEnumerable<WallItem> items) : base(items) { }

    static Identifier IMessage<WallItemsMsg>.Identifier => In.Items;

    static WallItemsMsg IParser<WallItemsMsg>.Parse(in PacketReader p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            var items = new WallItemsMsg();
            while (p.Available > 0)
                items.Add(WallItem.ParseOriginsString(p.ReadString()));
            return items;
        }
        else
        {
            var ownerDictionary = new Dictionary<Id, string>();

            int n = p.ReadLength();
            for (int i = 0; i < n; i++)
                ownerDictionary.Add(p.ReadId(), p.ReadString());

            n = p.ReadLength();
            var items = new WallItemsMsg(n);
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
        if (p.Client is ClientType.Shockwave)
        {
            foreach (var item in this)
                p.WriteString(item.ToOriginsString());
        }
        else
        {
            var ownerIds = new HashSet<Id>();
            var ownerDictionary = this
                .Where(x => ownerIds.Add(x.OwnerId))
                .ToDictionary(
                    key => key.OwnerId,
                    val => val.OwnerName
                );

            p.WriteLength((Length)ownerDictionary.Count);
            foreach (var pair in ownerDictionary)
            {
                p.WriteId(pair.Key);
                p.WriteString(pair.Value);
            }

            p.WriteLength((Length)Count);
            foreach (var item in this)
                p.Compose(item);
        }
    }
}
