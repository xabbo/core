using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when clicking a tile in a room to walk to.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.MoveAvatar"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.MOVE"/></item>
/// </list>
/// </summary>
/// <param name="Point">The coordinates of the tile.</param>
public sealed record WalkMsg(Point Point) : IMessage<WalkMsg>
{
    static Identifier IMessage<WalkMsg>.Identifier => Out.MoveAvatar;

    public WalkMsg(int x, int y) : this(new Point(x, y)) { }

    public int X => Point.X;
    public int Y => Point.Y;

    static WalkMsg IParser<WalkMsg>.Parse(in PacketReader p) => p.Client switch
    {
        not ClientType.Shockwave => new(p.ReadInt(), p.ReadInt()),
        ClientType.Shockwave => new(p.ReadB64(), p.ReadB64()),
    };

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.WriteB64((B64)Point.X);
            p.WriteB64((B64)Point.Y);
        }
        else
        {
            p.WriteInt(Point.X);
            p.WriteInt(Point.Y);
        }
    }
}
