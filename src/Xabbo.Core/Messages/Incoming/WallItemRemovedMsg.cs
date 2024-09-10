using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <param name="Id">The wall item ID.</param>
public sealed record WallItemRemovedMsg(Id Id) : IMessage<WallItemRemovedMsg>
{
    static Identifier IMessage<WallItemRemovedMsg>.Identifier => In.ItemRemove;

    public static WallItemRemovedMsg Parse(in PacketReader p)
    {
        Id id;
        switch (p.Client)
        {
            case ClientType.Unity:
                id = p.ReadId();
                break;
            case ClientType.Flash or ClientType.Shockwave:
                string strId = p.Client is ClientType.Flash ? p.ReadString() : p.ReadContent();
                if (!Id.TryParse(strId, out id))
                    throw new FormatException($"Failed to parse Id in WallItemRemovedMsg: '{strId}'.");
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
        return new(id);
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
                p.WriteContent(Id.ToString());
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }
}