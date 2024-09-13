using Xabbo.Messages;
using Xabbo.Messages.Shockwave;

namespace Xabbo.Core.Messages.Incoming.Origins;

public sealed record FriendAddedMsg(Friend Friend) : IMessage<FriendAddedMsg>
{
    static bool IMessage<FriendAddedMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<FriendAddedMsg>.Identifier => In.ADD_BUDDY;

    static FriendAddedMsg IParser<FriendAddedMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ~ClientType.Shockwave);
        return new(p.Parse<Friend>());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ~ClientType.Shockwave);
        p.Compose(Friend);
    }
}