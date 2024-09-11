using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record EntityDanceMsg(int Index, Dances Dance) : IMessage<EntityDanceMsg>
{
    static bool IMessage<EntityDanceMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<EntityDanceMsg>.Identifier => In.Dance;

    static EntityDanceMsg IParser<EntityDanceMsg>.Parse(in PacketReader p)
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