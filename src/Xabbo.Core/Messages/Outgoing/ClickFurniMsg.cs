using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record ClickFurniMsg(ItemType Type, Id Id, int State = 0) : IMessage<ClickFurniMsg>
{
    static Identifier IMessage<ClickFurniMsg>.Identifier => Out.ClickFurni;
    static bool IMessage<ClickFurniMsg>.UseTargetedIdentifiers => true;

    Identifier IMessage<ClickFurniMsg>.GetIdentifier(ClientType client) => client switch
    {
        ClientType.Flash => Out.ClickFurni,
        _ => throw new UnsupportedClientException(client),
    };

    public static ClickFurniMsg Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        // A negative ID is used to indicate a wall item.
        Id id = p.ReadId();
        ItemType type = id < 0 ? ItemType.Wall : ItemType.Floor;
        if (id < 0)
            id = -id;
        int state = p.ReadInt();

        return new ClickFurniMsg(type, id, state);
    }

    public void Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        if (Id < 0)
            throw new Exception($"Id cannot be negative when composing ClickFurniMsg: {Id}.");
        p.WriteId(Type switch
        {
            ItemType.Floor => Id,
            ItemType.Wall => -Id,
            _ => throw new Exception($"Invalid ItemType when composing ClickFurniMsg: {Type}."),
        });
        p.WriteInt(State);
    }
}