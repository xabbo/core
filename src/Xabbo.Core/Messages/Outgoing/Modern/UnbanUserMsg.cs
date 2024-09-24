using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing.Modern;

public sealed record UnbanUserMsg(Id RoomId, Id UserId) : IMessage<UnbanUserMsg>
{
    static Identifier IMessage<UnbanUserMsg>.Identifier => Out.UnbanUserFromRoom;
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
