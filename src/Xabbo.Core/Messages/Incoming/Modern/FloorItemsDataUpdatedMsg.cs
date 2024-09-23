using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

public sealed record FloorItemsDataUpdatedMsg(List<(Id Id, ItemData Data)> Updates) : IMessage<FloorItemsDataUpdatedMsg>
{
    static ClientType IMessage<FloorItemsDataUpdatedMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<FloorItemsDataUpdatedMsg>.Identifier => In.ObjectsDataUpdate;

    static FloorItemsDataUpdatedMsg IParser<FloorItemsDataUpdatedMsg>.Parse(in PacketReader p)
    {
        int n = p.ReadLength();
        List<(Id, ItemData)> updates = new(n);
        for (int i = 0; i < n; i++)
            updates.Add((p.ReadId(), p.Parse<ItemData>()));

        return new FloorItemsDataUpdatedMsg(updates);
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteLength(Updates.Count);
        foreach (var (id, itemData) in Updates)
        {
            p.WriteId(id);
            p.Compose(itemData);
        }
    }
}
