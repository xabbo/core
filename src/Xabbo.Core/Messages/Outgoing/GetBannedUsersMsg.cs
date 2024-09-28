using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when requesting the list of users banned from a room.
/// <para/>
/// Request for <see cref="BannedUsersMsg"/>. Returns the list of banned users as an array of <see cref="IdName"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.GetBannedUsersFromRoom"/></item>
/// </list>
/// </summary>
/// <param name="RoomId">The ID of the room to request banned users for.</param>
public sealed record GetBannedUsersMsg(Id RoomId) : IRequestMessage<GetBannedUsersMsg, BannedUsersMsg, IdName[]>
{
    static ClientType IMessage<GetBannedUsersMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GetBannedUsersMsg>.Identifier => Out.GetBannedUsersFromRoom;
    bool IRequestFor<BannedUsersMsg>.MatchResponse(BannedUsersMsg msg) => msg.RoomId == RoomId;
    IdName[] IResponseData<BannedUsersMsg, IdName[]>.GetData(BannedUsersMsg msg) => [.. msg.Users];
    static GetBannedUsersMsg IParser<GetBannedUsersMsg>.Parse(in PacketReader p) => new(p.ReadId());
    void IComposer.Compose(in PacketWriter p) => p.WriteId(RoomId);
}
