using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when placing a floor item in a room.
/// <para/>
/// Supported clients: <see cref="ClientType.Flash"/>, <see cref="ClientType.Shockwave"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.PlaceObject"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.PLACESTUFF"/></item>
/// </list>
/// </summary>
/// <param name="ItemId">The ID of the floor item to place.</param>
/// <param name="Location">The location to place the item at.</param>
/// <param name="Direction">The direction to place the item in.</param>
/// <param name="SizeX">The size of the object on the X axis. Applies to the <see cref="ClientType.Origins"/> client.</param>
/// <param name="SizeY">The size of the object on the Y axis. Applies to the <see cref="ClientType.Origins"/> client.</param>
public sealed record PlaceFloorItemMsg(
    Id ItemId, Point Location, int Direction, int SizeX = 1, int SizeY = 1
)
    : IMessage<PlaceFloorItemMsg>
{
    private static int ExpectedFieldCount(ClientType client) => client switch
    {
        not ClientType.Shockwave => 4,
        ClientType.Shockwave => 6,
    };

    static Identifier IMessage<PlaceFloorItemMsg>.Identifier => Out.PlaceObject;

    /// <summary>
    /// Gets the X coordinate.
    /// </summary>
    public int X => Location.X;

    /// <summary>
    /// Gets the Y coordinate.
    /// </summary>
    public int Y => Location.Y;

    public PlaceFloorItemMsg(Id itemId, int X, int Y, int direction, int sizeX = 1, int sizeY = 1)
        : this(itemId, (X, Y), direction, sizeX, sizeY)
    { }

    static bool IMessage<PlaceFloorItemMsg>.Match(in PacketReader p)
    {
        string content = p.Client switch
        {
            not ClientType.Shockwave => p.ReadString(),
            ClientType.Shockwave => p.ReadContent(),
        };
        int index = content.IndexOf(' ');
        if (index < 0 || (index + 1) >= content.Length) return false;
        return content[index + 1] != ':';
    }

    static PlaceFloorItemMsg IParser<PlaceFloorItemMsg>.Parse(in PacketReader p)
    {
        string content = p.Client switch
        {
            not ClientType.Shockwave => p.ReadString(),
            ClientType.Shockwave => p.ReadContent(),
        };

        string[] fields = content.Split();
        if (fields.Length != ExpectedFieldCount(p.Client))
            throw new Exception("Unexpected field count in PlaceFloorItemMsg.");

        if (!Id.TryParse(fields[0], out Id itemId))
            throw new Exception($"Failed to parse ItemId in PlaceFloorItemMsg: '{fields[0]}'.");
        if (!int.TryParse(fields[1], out int x))
            throw new Exception($"Failed to parse X in PlaceFloorItemMsg: '{fields[1]}'.");
        if (!int.TryParse(fields[2], out int y))
            throw new Exception($"Failed to parse Y in PlaceFloorItemMsg: '{fields[2]}'.");

        int sizeX = 1, sizeY = 1;
        if (p.Client is ClientType.Shockwave)
        {
            if (!int.TryParse(fields[3], out sizeX))
                throw new Exception($"Failed to parse SizeX in PlaceFloorItemMsg: '{fields[3]}'.");
            if (!int.TryParse(fields[4], out sizeY))
                throw new Exception($"Failed to parse SizeY in PlaceFloorItemMsg: '{fields[4]}'.");
        }

        if (!int.TryParse(fields[^1], out int direction))
            throw new Exception($"Failed to parse Direction in PlaceFloorItemMsg: '{fields[^1]}'.");

        return new PlaceFloorItemMsg(itemId, x, y, direction, sizeX, sizeY);
    }

    void IComposer.Compose(in PacketWriter p)
    {
        switch (p.Client)
        {
            case ClientType.Flash:
                p.WriteString($"{ItemId} {X} {Y} {Direction}");
                break;
            case ClientType.Shockwave:
                p.WriteContent($"{ItemId} {X} {Y} {SizeX} {SizeY} {Direction}");
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }
}