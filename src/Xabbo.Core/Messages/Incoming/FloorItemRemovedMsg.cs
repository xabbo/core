using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <param name="Id">The floor item ID.</param>
/// <param name="Item">The floor item instance. Only available on Shockwave.</param>
public sealed record FloorItemRemovedMsg(Id Id, FloorItem? Item = null) : IMessage<FloorItemRemovedMsg>
{
    static Identifier IMessage<FloorItemRemovedMsg>.Identifier => In.ObjectRemove;

    public static FloorItemRemovedMsg Parse(in PacketReader p)
    {
        Id id;
        FloorItem? item = null;
        switch (p.Client)
        {
            case ClientType.Unity:
                id = p.ReadId();
                break;
            case ClientType.Flash:
                string strId = p.ReadString();
                if (!Id.TryParse(strId, out id))
                    throw new FormatException($"Failed to parse Id in FloorItemRemovedMsg: '{strId}'.");
                break;
            case ClientType.Shockwave:
                item = p.Parse<FloorItem>();
                id = item.Id;
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
        return new(id, item);
    }

    public void Compose(in PacketWriter p)
    {
        switch (p.Client)
        {
            case ClientType.Unity:
                p.WriteId(Id);
                break;
            case ClientType.Flash:
                p.WriteString(Id.ToString());
                break;
            case ClientType.Shockwave:
                // Shockwave sends the entire structure, but the client only cares about the ID.
                if (Item is null)
                    p.WriteString(Id.ToString());
                else
                    p.Compose(Item);
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }
}