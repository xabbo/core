using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record AvatarHandItemMsg(int Index, int Item) : IMessage<AvatarHandItemMsg>
{
    static bool IMessage<AvatarHandItemMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<AvatarHandItemMsg>.Identifier => In.CarryObject;

    static AvatarHandItemMsg IParser<AvatarHandItemMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        return new(p.ReadInt(), p.ReadInt());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        p.WriteInt(Index);
        p.WriteInt(Item);
    }
}