using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class SlideEntity : IParserComposer<SlideEntity>
{
    public int Index { get; set; }
    public float FromZ { get; set; }
    public float ToZ { get; set; }

    public SlideEntity() { }

    private SlideEntity(in PacketReader p)
    {
        Index = p.ReadInt();
        FromZ = p.ReadFloat();
        ToZ = p.ReadFloat();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteFloat(FromZ);
        p.WriteFloat(ToZ);
    }

    static SlideEntity IParser<SlideEntity>.Parse(in PacketReader p) => new(p);
}
