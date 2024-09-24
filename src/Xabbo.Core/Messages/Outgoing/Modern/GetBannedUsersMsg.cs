using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Request for <see cref="BannedUsersMsg"/>.
/// </summary>
public sealed record GetBannedUsersMsg(Id RoomId) : IRequestMessage<GetBannedUsersMsg, BannedUsersMsg>
{
    static ClientType IMessage<GetBannedUsersMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GetBannedUsersMsg>.Identifier => Out.GetBannedUsersFromRoom;
    static GetBannedUsersMsg IParser<GetBannedUsersMsg>.Parse(in PacketReader p) => new(p.ReadId());
    void IComposer.Compose(in PacketWriter p) => p.WriteId(RoomId);
}
