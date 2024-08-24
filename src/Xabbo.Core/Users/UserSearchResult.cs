using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class UserSearchResult : IComposer, IParser<UserSearchResult>
{
    public static UserSearchResult Parse(in PacketReader packet) => new(in packet);

    public Id Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Motto { get; set; } = string.Empty;
    public bool Online { get; set; }
    public bool UnknownBoolA { get; set; }
    public string UnknownStringA { get; set; } = string.Empty;
    public Id UnknownLongA { get; set; }
    public string Figure { get; set; } = string.Empty;
    public string RealName { get; set; } = string.Empty;

    public UserSearchResult() { }

    private UserSearchResult(in PacketReader packet)
    {
        Id = packet.Read<Id>();
        Name = packet.Read<string>();
        Motto = packet.Read<string>();
        Online = packet.Read<bool>();
        UnknownBoolA = packet.Read<bool>();
        UnknownStringA = packet.Read<string>();
        UnknownLongA = packet.Read<Id>();
        Figure  = packet.Read<string>();
        RealName = packet.Read<string>();
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write(Name);
        p.Write(Motto);
        p.Write(Online);
        p.Write(UnknownBoolA);
        p.Write(UnknownStringA);
        p.Write(UnknownLongA);
        p.Write(Figure);
        p.Write(RealName);
    }
}
