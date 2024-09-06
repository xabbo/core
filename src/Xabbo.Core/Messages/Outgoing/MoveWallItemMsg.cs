using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record MoveWallItemMsg(Id Id, WallLocation Location) : IMessage<MoveWallItemMsg>
{
    static Identifier IMessage<MoveWallItemMsg>.Identifier => Out.MoveWallItem;

    static MoveWallItemMsg IParser<MoveWallItemMsg>.Parse(in PacketReader p) => new(p.ReadId(), (WallLocation)p.ReadString());

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString(Location.ToString());
    }
}