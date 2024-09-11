using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record EntityEffectMsg(int Index, int Effect, int Delay = 0) : IMessage<EntityEffectMsg>
{
    static bool IMessage<EntityEffectMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<EntityEffectMsg>.Identifier => In.AvatarEffect;

    static EntityEffectMsg IParser<EntityEffectMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        return new(p.ReadInt(), p.ReadInt(), p.ReadInt());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        p.WriteInt(Index);
        p.WriteInt(Effect);
        p.WriteInt(Delay);
    }
}