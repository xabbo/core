using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record EntityActionMsg(int Index, Actions Action) : IMessage<EntityActionMsg>
{
    static bool IMessage<EntityActionMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<EntityActionMsg>.Identifier => In.Expression;

    static EntityActionMsg IParser<EntityActionMsg>.Parse(in PacketReader p)
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