using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class UserSearchResult : IComposer, IParser<UserSearchResult>
{
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

    private UserSearchResult(in PacketReader p)
    {
        Id = p.Read<Id>();
        Name = p.Read<string>();
        Motto = p.Read<string>();
        Online = p.Read<bool>();
        UnknownBoolA = p.Read<bool>();
        UnknownStringA = p.Read<string>();
        UnknownLongA = p.Read<Id>();
        Figure  = p.Read<string>();
        RealName = p.Read<string>();
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

    public static UserSearchResult Parse(in PacketReader p) => new(in p);
}
