using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class SlideAvatar : IParserComposer<SlideAvatar>
{
    public int Index { get; set; }
    public float FromZ { get; set; }
    public float ToZ { get; set; }

    public SlideAvatar() { }

    private SlideAvatar(in PacketReader p)
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

    static SlideAvatar IParser<SlideAvatar>.Parse(in PacketReader p) => new(p);
}
