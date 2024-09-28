using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when moving a wall item.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.MoveWallItem"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.MOVEITEM"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the wall item to move.</param>
/// <param name="Location">The location to move the wall item to.</param>
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
