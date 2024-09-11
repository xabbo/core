using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record EntityTypingMsg(int Index, bool Typing) : IMessage<EntityTypingMsg>
{
    static bool IMessage<EntityTypingMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<EntityTypingMsg>.Identifier => In.UserTyping;

    static EntityTypingMsg IParser<EntityTypingMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        return new(p.ReadInt(), p.ReadInt() != 0);
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        p.WriteInt(Index);
        p.WriteInt(Typing ? 1 : 0);
    }
}