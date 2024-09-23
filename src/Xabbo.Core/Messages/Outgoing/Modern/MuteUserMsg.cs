using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing.Modern;

public sealed record MuteUserMsg(Id Id, Id RoomId, int Minutes) : IMessage<MuteUserMsg>
{
    static ClientType IMessage<MuteUserMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<MuteUserMsg>.Identifier => Out.MuteUser;

    public MuteUserMsg(IUser user, Id roomId, int minutes) : this(user.Id, roomId, minutes) { }

    static MuteUserMsg IParser<MuteUserMsg>.Parse(in PacketReader p) => new(p.ReadId(), p.ReadId(), p.ReadInt());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteId(RoomId);
        p.WriteInt(Minutes);
    }
}
