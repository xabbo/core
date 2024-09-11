using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record EntityHandItemMsg(int Index, int Item) : IMessage<EntityHandItemMsg>
{
    static bool IMessage<EntityHandItemMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<EntityHandItemMsg>.Identifier => In.CarryObject;

    static EntityHandItemMsg IParser<EntityHandItemMsg>.Parse(in PacketReader p)
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