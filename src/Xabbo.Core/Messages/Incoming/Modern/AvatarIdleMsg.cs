using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record AvatarIdleMsg(int Index, bool Idle) : IMessage<AvatarIdleMsg>
{
    static ClientType IMessage<AvatarIdleMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<AvatarIdleMsg>.Identifier => In.Sleep;

    static AvatarIdleMsg IParser<AvatarIdleMsg>.Parse(in PacketReader p) => new(p.ReadInt(), p.ReadBool());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteBool(Idle);
    }
}