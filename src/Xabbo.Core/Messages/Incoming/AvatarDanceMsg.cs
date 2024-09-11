using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record AvatarDanceMsg(int Index, Dances Dance) : IMessage<AvatarDanceMsg>
{
    static bool IMessage<AvatarDanceMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<AvatarDanceMsg>.Identifier => In.Dance;

    static AvatarDanceMsg IParser<AvatarDanceMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        return new(p.ReadInt(), (Dances)p.ReadInt());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        p.WriteInt(Index);
        p.WriteInt((int)Dance);
    }
}