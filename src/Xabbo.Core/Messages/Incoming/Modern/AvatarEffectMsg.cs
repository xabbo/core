using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

public sealed record AvatarEffectMsg(int Index, int Effect, int Delay = 0) : IMessage<AvatarEffectMsg>
{
    static ClientType IMessage<AvatarEffectMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<AvatarEffectMsg>.Identifier => In.AvatarEffect;

    static AvatarEffectMsg IParser<AvatarEffectMsg>.Parse(in PacketReader p) => new(p.ReadInt(), p.ReadInt(), p.ReadInt());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteInt(Effect);
        p.WriteInt(Delay);
    }
}