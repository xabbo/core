using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a badge ID and code.
/// </summary>
public sealed class Badge : IParserComposer<Badge>
{
    public int Id { get; set; }
    public string Code { get; set; } = "";

    public Badge() { }

    public Badge(int id, string code)
    {
        Id = id;
        Code = code;
    }

    private Badge(in PacketReader p)
    {
        Id = p.ReadInt();
        Code = p.ReadString();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Id);
        p.WriteString(Code);
    }

    static Badge IParser<Badge>.Parse(in PacketReader p) => new(p);
}
