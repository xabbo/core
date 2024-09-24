using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

public sealed record UserUnbannedMsg(Id RoomId, Id UserId) : IMessage<UserUnbannedMsg>
{
    static ClientType IMessage<UserUnbannedMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<UserUnbannedMsg>.Identifier => In.UserUnbannedFromRoom;
    static UserUnbannedMsg IParser<UserUnbannedMsg>.Parse(in PacketReader p) => new(
        RoomId: p.ReadId(),
        UserId: p.ReadId()
    );
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(RoomId);
        p.WriteId(UserId);
    }
}
