using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when unbanning a user from a room.
/// <para/>
/// The server may respond with <see cref="UserUnbannedMsg"/> if the user was unbanned.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.UnbanUserFromRoom"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the user to unban.</param>
/// <param name="RoomId">The ID of the room to unban the user from.</param>
public sealed record UnbanUserMsg(Id Id, Id RoomId) : IRequestMessage<UnbanUserMsg, UserUnbannedMsg>
{
    static Identifier IMessage<UnbanUserMsg>.Identifier => Out.UnbanUserFromRoom;

    bool IRequestFor<UserUnbannedMsg>.MatchResponse(UserUnbannedMsg msg)
        => msg.RoomId == RoomId && msg.UserId == Id;

    static UnbanUserMsg IParser<UnbanUserMsg>.Parse(in PacketReader p) => new(
        Id: p.ReadId(),
        RoomId: p.ReadId()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteId(RoomId);
    }
}
