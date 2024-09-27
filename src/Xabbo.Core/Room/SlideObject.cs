using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a floor item being moved by a roller.
/// </summary>
public sealed class SlideObject : IParserComposer<SlideObject>
{
    /// <summary>
    /// The ID of the floor item.
    /// </summary>
    public Id Id { get; set; }

    /// <summary>
    /// The height the object is sliding from.
    /// </summary>
    public float FromZ { get; set; }

    /// <summary>
    /// The height the object is sliding to.
    /// </summary>
    public float ToZ { get; set; }

    /// <summary>
    /// Constructs a new <see cref="SlideObject"/>.
    /// </summary>
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
