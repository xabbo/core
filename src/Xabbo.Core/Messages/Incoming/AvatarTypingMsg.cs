using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record AvatarTypingMsg(int Index, bool Typing) : IMessage<AvatarTypingMsg>
{
    static bool IMessage<AvatarTypingMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<AvatarTypingMsg>.Identifier => In.UserTyping;

    static AvatarTypingMsg IParser<AvatarTypingMsg>.Parse(in PacketReader p)
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