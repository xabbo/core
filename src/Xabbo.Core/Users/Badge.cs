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

    private Badge(in PacketReader packet)
    {
        Id = packet.Read<int>();
        Code = packet.Read<string>();
    }

    public static Badge Parse(in PacketReader packet) => new(packet);

    public void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write(Code);
    }

}
