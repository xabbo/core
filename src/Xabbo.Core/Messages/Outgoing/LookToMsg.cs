using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when looking towards a tile.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.LookTo"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.LOOKTO"/></item>
/// </list>
/// </summary>
/// <param name="Point">The point to look towards.</param>
public sealed record LookToMsg(Point Point) : IMessage<LookToMsg>
{
    public static Identifier Identifier => Out.LookTo;

    public LookToMsg(int X, int Y) : this(new Point(X, Y)) { }

    /// <summary>
    /// Gets the X coordinate.
    /// </summary>
    public int X => Point.X;

    /// <summary>
    /// Gets the Y coordinate.
    /// </summary>
    public int Y => Point.Y;

    static LookToMsg IParser<LookToMsg>.Parse(in PacketReader p)
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

    void IComposer.Compose(in PacketWriter p)
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
