using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record MuteUserMsg(Id UserId, Id RoomId, int Minutes) : IMessage<MuteUserMsg>
{
    static bool IMessage<MuteUserMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<MuteUserMsg>.Identifier => Out.MuteUser;

    static MuteUserMsg IParser<MuteUserMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        return new MuteUserMsg(p.ReadId(), p.ReadId(), p.ReadInt());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);
        p.WriteId(UserId);
        p.WriteId(RoomId);
        p.WriteInt(Minutes);
    }

}