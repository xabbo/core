using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record AvatarIdleMsg(int Index, bool Idle) : IMessage<AvatarIdleMsg>
{
    static bool IMessage<AvatarIdleMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<AvatarIdleMsg>.Identifier => In.Sleep;

    static AvatarIdleMsg IParser<AvatarIdleMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        return new(p.ReadInt(), p.ReadBool());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        p.WriteInt(Index);
        p.WriteBool(Idle);
    }
}