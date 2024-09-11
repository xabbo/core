using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class SlideObject : IParserComposer<SlideObject>
{
    public Id Id { get; set; }
    public float FromZ { get; set; }
    public float ToZ { get; set; }

    public SlideObject() { }

    private SlideObject(in PacketReader p)
    {
        Id = p.ReadId();
        FromZ = p.ReadFloat();
        ToZ = p.ReadFloat();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteFloat(FromZ);
        p.WriteFloat(ToZ);
    }

    static SlideObject IParser<SlideObject>.Parse(in PacketReader p) => new(p);
}
