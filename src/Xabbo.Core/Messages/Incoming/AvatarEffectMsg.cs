using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record AvatarEffectMsg(int Index, int Effect, int Delay = 0) : IMessage<AvatarEffectMsg>
{
    static bool IMessage<AvatarEffectMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<AvatarEffectMsg>.Identifier => In.AvatarEffect;

    static AvatarEffectMsg IParser<AvatarEffectMsg>.Parse(in PacketReader p)
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