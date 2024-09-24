using System.Collections.Generic;
using Xabbo.Core.Events;
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Response for <see cref="Outgoing.Modern.GetBannedUsersMsg"/>.
/// </summary>
public sealed record BannedUsersMsg(Id RoomId, List<(Id Id, string Name)> Users) : IMessage<BannedUsersMsg>
{
    static ClientType IMessage<BannedUsersMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<BannedUsersMsg>.Identifier => In.BannedUsersFromRoom;
    static BannedUsersMsg IParser<BannedUsersMsg>.Parse(in PacketReader p)
    {
        Id roomId = p.ReadId();
        var list = new List<(Id, string)>(p.ReadLength());
        for (int i = 0; i < list.Capacity; i++)
            list.Add((p.ReadId(), p.ReadString()));
        return new(roomId, list);
    }
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(RoomId);
        p.WriteLength(Users.Count);
        foreach (var (id, name) in Users)
        {
            p.WriteId(id);
            p.WriteString(name);
        }
    }
}
