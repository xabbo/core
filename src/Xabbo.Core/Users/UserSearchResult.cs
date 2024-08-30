using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class UserSearchResult : IParserComposer<UserSearchResult>
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
        Id = p.ReadId();
        Name = p.ReadString();
        Motto = p.ReadString();
        Online = p.ReadBool();
        UnknownBoolA = p.ReadBool();
        UnknownStringA = p.ReadString();
        UnknownLongA = p.ReadId();
        Figure  = p.ReadString();
        RealName = p.ReadString();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString(Name);
        p.WriteString(Motto);
        p.WriteBool(Online);
        p.WriteBool(UnknownBoolA);
        p.WriteString(UnknownStringA);
        p.WriteId(UnknownLongA);
        p.WriteString(Figure);
        p.WriteString(RealName);
    }

    static UserSearchResult IParser<UserSearchResult>.Parse(in PacketReader p) => new(in p);
}
