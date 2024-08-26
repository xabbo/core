using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class Badge : IComposer, IParser<Badge>
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;

    public Badge() { }

    public Badge(int id, string code)
    {
        Id = id;
        Code = code;
    }

    private Badge(in PacketReader p)
    {
        Id = p.Read<int>();
        Code = p.Read<string>();
    }

    public static Badge Parse(in PacketReader p) => new(p);

    public void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write(Code);
    }

}
