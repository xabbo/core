using System;
using Xabbo.Messages;

namespace Xabbo.Core;

public class UserSearchResult
{
    public static UserSearchResult Parse(IReadOnlyPacket packet) => new UserSearchResult(packet);

    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Motto { get; set; } = string.Empty;
    public bool Online { get; set; }
    public bool UnknownBoolA { get; set; }
    public string UnknownStringA { get; set; } = string.Empty;
    public long UnknownLongA { get; set; }
    public string Figure { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;

    public UserSearchResult() { }

    protected UserSearchResult(IReadOnlyPacket packet)
    {
        Id = packet.ReadLegacyLong();
        Name = packet.ReadString();
        Motto = packet.ReadString();
        Online = packet.ReadBool();
        UnknownBoolA = packet.ReadBool();
        UnknownStringA = packet.ReadString();
        UnknownLongA = packet.ReadLegacyLong();
        Figure  = packet.ReadString();
        RealName = packet.ReadString();
    }
}
