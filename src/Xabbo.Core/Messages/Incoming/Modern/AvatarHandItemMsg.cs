using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

public sealed record AvatarHandItemMsg(int Index, int Item) : IMessage<AvatarHandItemMsg>
{
    static ClientType IMessage<AvatarHandItemMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<AvatarHandItemMsg>.Identifier => In.CarryObject;

    static AvatarHandItemMsg IParser<AvatarHandItemMsg>.Parse(in PacketReader p) => new(p.ReadInt(), p.ReadInt());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteInt(Item);
    }
}
