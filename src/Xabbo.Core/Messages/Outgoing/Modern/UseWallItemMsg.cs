using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing.Modern;

public sealed record UseWallItemMsg(Id Id, int State = 0) : IMessage<UseWallItemMsg>
{
    static ClientType IMessage<UseWallItemMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<UseWallItemMsg>.Identifier => Out.UseWallItem;

    static UseWallItemMsg IParser<UseWallItemMsg>.Parse(in PacketReader p) => new(p.ReadId(), p.ReadInt());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteInt(State);
    }
}