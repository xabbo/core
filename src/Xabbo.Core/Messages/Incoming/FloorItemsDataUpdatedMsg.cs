using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record FloorItemsDataUpdatedMsg(List<(Id Id, ItemData Data)> Updates) : IMessage<FloorItemsDataUpdatedMsg>
{
    static Identifier IMessage<FloorItemsDataUpdatedMsg>.Identifier => In.ObjectsDataUpdate;

    static bool IMessage<FloorItemsDataUpdatedMsg>.UseTargetedIdentifiers => true;

    static FloorItemsDataUpdatedMsg IParser<FloorItemsDataUpdatedMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        int n = p.ReadLength();
        List<(Id, ItemData)> updates = new(n);
        for (int i = 0; i < n; i++)
            updates.Add((p.ReadId(), p.Parse<ItemData>()));

        return new FloorItemsDataUpdatedMsg(updates);
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        p.WriteLength(Updates.Count);
        foreach (var (id, itemData) in Updates)
        {
            p.WriteId(id);
            p.Compose(itemData);
        }
    }
}