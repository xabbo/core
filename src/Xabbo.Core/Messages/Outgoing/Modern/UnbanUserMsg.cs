using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Messages.Outgoing.Modern;

public sealed record UnbanUserMsg(Id RoomId, Id UserId) : IRequestMessage<UnbanUserMsg, UserUnbannedMsg>
{
    static Identifier IMessage<UnbanUserMsg>.Identifier => Out.UnbanUserFromRoom;
    bool IRequestFor<UserUnbannedMsg>.MatchResponse(UserUnbannedMsg msg)
        => msg.RoomId == RoomId && msg.UserId == UserId;
    static UnbanUserMsg IParser<UnbanUserMsg>.Parse(in PacketReader p) => new(
        UserId: p.ReadId(),
        RoomId: p.ReadId()
    );
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(UserId);
        p.WriteId(RoomId);
    }
}
