using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when moving a floor item.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.MoveObject"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.MOVESTUFF"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the floor item to move.</param>
/// <param name="Location">The location to move the floor item to.</param>
/// <param name="Direction">The direction to set the floor item to.</param>
public sealed record MoveFloorItemMsg(Id Id, Point Location, int Direction) : IMessage<MoveFloorItemMsg>
{
    const int ExpectedFieldCount = 4;

    static Identifier IMessage<MoveFloorItemMsg>.Identifier => Out.MoveObject;

    /// <summary>
    /// Gets the X coordinate.
    /// </summary>
    public int X => Location.X;

    /// <summary>
    /// Gets the Y coordinate.
    /// </summary>
    public int Y => Location.Y;

    public MoveFloorItemMsg(Id Id, int X, int Y, int Direction)
        : this(Id, (X, Y), Direction)
    { }

    static MoveFloorItemMsg IParser<MoveFloorItemMsg>.Parse(in PacketReader p)
    {
        Id id;
        int x, y, direction;

        if (p.Client == ClientType.Shockwave)
        {
            string[] fields = p.ReadContent().Split();
            if (fields.Length != ExpectedFieldCount)
                throw new Exception($"Unexpected field count when parsing MoveFloorItemMsg: {fields.Length}. Expected: {ExpectedFieldCount}.");
            if (!Id.TryParse(fields[0], out id))
                throw new FormatException($"Failed to parse Id in MoveFloorItemMsg: '{fields[0]}'.");
            if (!int.TryParse(fields[1], out x))
                throw new FormatException($"Failed to parse X in MoveFloorItemMsg: '{fields[1]}'.");
            if (!int.TryParse(fields[2], out y))
                throw new FormatException($"Failed to parse Y in MoveFloorItemMsg: '{fields[2]}'.");
            if (!int.TryParse(fields[3], out direction))
                throw new FormatException($"Failed to parse Direction in MoveFloorItemMsg: '{fields[3]}'.");
        }
        else
        {
            id = p.ReadId();
            x = p.ReadInt();
            y = p.ReadInt();
            direction = p.ReadInt();
        }

        return new MoveFloorItemMsg(id, x, y, direction);
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client == ClientType.Shockwave)
        {
            p.WriteContent($"{Id} {X} {Y} {Direction}");
        }
        else
        {
            p.WriteId(Id);
            p.WriteInt(X);
            p.WriteInt(Y);
            p.WriteInt(Direction);
        }
    }
}
