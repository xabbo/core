using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents an avatar being moved by a roller.
/// </summary>
public sealed class SlideAvatar : IParserComposer<SlideAvatar>
{
    /// <summary>
    /// The index of the avatar.
    /// </summary>
    public int Index { get; set; }

    /// <summary>
    /// The height the avatar is sliding from.
    /// </summary>
    public float FromZ { get; set; }

    /// <summary>
    /// The height the avatar is sliding to.
    /// </summary>
    public float ToZ { get; set; }

    /// <summary>
    /// Constructs a new <see cref="SlideAvatar"/>.
    /// </summary>
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
