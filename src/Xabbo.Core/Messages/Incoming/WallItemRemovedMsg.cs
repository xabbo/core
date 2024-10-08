using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <param name="Id">The wall item ID.</param>
/// <param name="PickerId">The ID of the user who picked up the item.</param>
public sealed record WallItemRemovedMsg(Id Id, Id PickerId = default) : IMessage<WallItemRemovedMsg>
{
    static Identifier IMessage<WallItemRemovedMsg>.Identifier => In.ItemRemove;

    public static WallItemRemovedMsg Parse(in PacketReader p)
    {
        Id id, pickerId = 0;

        if (p.Client is ClientType.Unity)
        {
            id = p.ReadId();
        }
        else
        {
            string strId = p.Client is ClientType.Flash ? p.ReadString() : p.ReadContent();
            if (!Id.TryParse(strId, out id))
                throw new FormatException($"Failed to parse Id in WallItemRemovedMsg: '{strId}'.");
        }

        if (p.Client is not ClientType.Shockwave)
            pickerId = p.ReadId();

        return new(id, pickerId);
    }

    public void Compose(in PacketWriter p)
    {
        switch (p.Client)
        {
            case ClientType.Unity or ClientType.Flash:
                if (p.Client is ClientType.Unity)
                    p.WriteId(Id);
                else
                    p.WriteString(Id.ToString());
                p.WriteId(PickerId);
                break;
            case ClientType.Shockwave:
                p.WriteContent(Id.ToString());
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }
}