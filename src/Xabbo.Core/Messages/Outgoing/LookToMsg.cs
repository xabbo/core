using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record LookToMsg(Point Point) : IMessage<LookToMsg>
{
    public static Identifier Identifier => Out.LookTo;

    public LookToMsg(int x, int y) : this(new Point(x, y)) { }

    public int X => Point.X;
    public int Y => Point.Y;

    public static LookToMsg Parse(in PacketReader p)
    {
        int x, y;
        if (p.Client is ClientType.Shockwave)
        {
            string[] fields = p.ReadContent().Split(' ');
            if (fields.Length != 2)
                throw new Exception($"Unexpected field count when parsing LookToMsg: {fields.Length}.");
            if (!int.TryParse(fields[0], out x))
                throw new Exception($"Failed to parse X in LookToMsg.");
            if (!int.TryParse(fields[1], out y))
                throw new Exception($"Failed to parse Y in LookToMsg.");
        }
        else
        {
            x = p.ReadInt();
            y = p.ReadInt();
        }

        return new LookToMsg((x, y));
    }

    public void Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.WriteContent($"{Point.X} {Point.Y}");
        }
        else
        {
            p.WriteInt(Point.X);
            p.WriteInt(Point.Y);
        }
    }
}