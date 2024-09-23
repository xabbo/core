using Xabbo.Messages;
using Xabbo.Messages.Shockwave;

namespace Xabbo.Core.Messages.Incoming.Origins;

public sealed record FriendAddedMsg(Friend Friend) : IMessage<FriendAddedMsg>
{
    static ClientType IMessage<FriendAddedMsg>.SupportedClients => ClientType.Origins;
    static Identifier IMessage<FriendAddedMsg>.Identifier => In.ADD_BUDDY;

    static FriendAddedMsg IParser<FriendAddedMsg>.Parse(in PacketReader p) => new(p.Parse<Friend>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Friend);
}
