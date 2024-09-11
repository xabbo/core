using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record EntityIdleMsg(int Index, bool Idle) : IMessage<EntityIdleMsg>
{
    static bool IMessage<EntityIdleMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<EntityIdleMsg>.Identifier => In.Sleep;

    static EntityIdleMsg IParser<EntityIdleMsg>.Parse(in PacketReader p)
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