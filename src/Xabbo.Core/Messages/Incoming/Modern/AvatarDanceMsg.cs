using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

public sealed record AvatarDanceMsg(int Index, Dances Dance) : IMessage<AvatarDanceMsg>
{
    static ClientType IMessage<AvatarDanceMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<AvatarDanceMsg>.Identifier => In.Dance;

    static AvatarDanceMsg IParser<AvatarDanceMsg>.Parse(in PacketReader p) => new(p.ReadInt(), (Dances)p.ReadInt());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteInt((int)Dance);
    }
}
