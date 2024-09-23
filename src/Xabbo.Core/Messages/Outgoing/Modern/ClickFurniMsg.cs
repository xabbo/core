using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing.Modern;

public sealed record ClickFurniMsg(ItemType Type, Id Id, int State = 0) : IMessage<ClickFurniMsg>
{
    static ClientType IMessage<ClickFurniMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<ClickFurniMsg>.Identifier => Out.ClickFurni;

    public static ClickFurniMsg Parse(in PacketReader p)
    {
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