using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record PlaceWallItemMsg(Id ItemId, WallLocation Location) : IMessage<PlaceWallItemMsg>
{
    static Identifier IMessage<PlaceWallItemMsg>.Identifier => Out.PlaceObject;

    static bool IMessage<PlaceWallItemMsg>.Match(in PacketReader p)
    {
        string content = p.ReadString();
        int index = content.IndexOf(' ');
        if (index < 0 || (index + 1) >= content.Length) return false;
        return content[index + 1] == ':';
    }

    static PlaceWallItemMsg IParser<PlaceWallItemMsg>.Parse(in PacketReader p)
    {
        string content = p.Client switch
        {
            ClientType.Flash => p.ReadString(),
            ClientType.Shockwave => p.ReadContent(),
            _ => throw new UnsupportedClientException(p.Client),
        };

        int i = content.IndexOf(' ');
        if (i < 0)
            throw new Exception("No separator in PlaceWallItemMsg.");

        return new PlaceWallItemMsg((Id)content[..i], (WallLocation)content[(i + 1)..]);
    }

    void IComposer.Compose(in PacketWriter p)
    {
        switch (p.Client)
        {
            case ClientType.Flash:
                p.WriteString($"{ItemId} {Location}");
                break;
            case ClientType.Shockwave:
                p.WriteContent($"{ItemId} {Location}");
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }
}