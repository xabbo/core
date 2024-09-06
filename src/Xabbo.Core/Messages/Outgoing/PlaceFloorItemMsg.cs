using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record PlaceFloorItemMsg(
    Id ItemId, int X, int Y, int Direction, int SizeX = 1, int SizeY = 1
)
    : IMessage<PlaceFloorItemMsg>
{
    private static int ExpectedFieldCount(ClientType client) => client switch
    {
        not ClientType.Shockwave => 4,
        ClientType.Shockwave => 6,
    };

    static Identifier IMessage<PlaceFloorItemMsg>.Identifier => Out.PlaceObject;

    static bool IMessage<PlaceFloorItemMsg>.Match(in PacketReader p)
    {
        string content = p.Client switch {
            not ClientType.Shockwave => p.ReadString(),
            ClientType.Shockwave => p.Content,
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
            ClientType.Shockwave => p.Content,
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
                p.Content = $"{ItemId} {X} {Y} {SizeX} {SizeY} {Direction}";
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }
}