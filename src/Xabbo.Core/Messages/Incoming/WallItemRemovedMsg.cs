using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a wall item is removed from the room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.ItemRemove"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.REMOVEITEM"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the wall item that was removed.</param>
/// <param name="PickerId">
/// The ID of the user who picked up the item.
/// Only available on <see cref="ClientType.Modern"/> clients.
/// </param>
public sealed record WallItemRemovedMsg(Id Id, Id PickerId = default) : IMessage<WallItemRemovedMsg>
{
    static Identifier IMessage<WallItemRemovedMsg>.Identifier => In.ItemRemove;

    static WallItemRemovedMsg IParser<WallItemRemovedMsg>.Parse(in PacketReader p)
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

    void IComposer.Compose(in PacketWriter p)
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
