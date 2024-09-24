using System.Collections.Generic;
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Response for <see cref="Outgoing.Modern.GetBannedUsersMsg"/>.
/// </summary>
public sealed record BannedUsersMsg(Id RoomId, List<IdName> Users) : IMessage<BannedUsersMsg>
{
    static ClientType IMessage<BannedUsersMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<BannedUsersMsg>.Identifier => In.BannedUsersFromRoom;
    static BannedUsersMsg IParser<BannedUsersMsg>.Parse(in PacketReader p) => new(
        RoomId: p.ReadId(),
        Users: [.. p.ParseArray<IdName>()]
    );
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(RoomId);
        p.ComposeArray(Users);
    }
}
