using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Represents a sticky note.
/// </summary>
public sealed class Sticky : IParserComposer<Sticky>
{
    public static readonly StickyColors Colors = new();

    public Id Id { get; set; }
    public string Color { get; set; } = "";
    public string Text { get; set; } = "";

    public static Sticky ParseString(Id id, string data)
    {
        string[] split = data.Split(' ', 2);
        return new Sticky
        {
            Id = id,
            Color = split.Length < 1 ? "" : split[0],
            Text = split.Length < 2 ? "" : split[1]
        };
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString($"{Color} {Text}");
    }

    static Sticky IParser<Sticky>.Parse(in PacketReader p)
    {
        var id = p.Client switch
        {
            ClientType.Unity => p.ReadId(),
            not ClientType.Unity => (Id)p.ReadString()
        };
        string data = p.ReadString();
        return ParseString(id, data);
    }
}
