using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record AvatarActionMsg(int Index, Actions Action) : IMessage<AvatarActionMsg>
{
    static bool IMessage<AvatarActionMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<AvatarActionMsg>.Identifier => In.Expression;

    static AvatarActionMsg IParser<AvatarActionMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        return new(p.ReadInt(), (Actions)p.ReadInt());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        p.WriteInt(Index);
        p.WriteInt((int)Action);
    }
}