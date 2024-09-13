using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

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