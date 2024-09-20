using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class Sticky : IParserComposer<Sticky>
{
    public static readonly StickyColors Colors = new();

    public Id Id { get; set; }
    public string Color { get; set; }
    public string Text { get; set; }

    public Sticky()
    {
        Color = "";
        Text = "";
    }

    private Sticky(in PacketReader p)
    {
        if (p.Client == ClientType.Shockwave)
            Id = (Id)p.ReadString();
        else
            Id = p.ReadId();
        string text = p.ReadString();
        int spaceIndex = text.IndexOf(' ');
        Color = text[0..6];
        if (spaceIndex == 6)
        {
            Text = text[7..];
        }
        else
        {
            Text = "";
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteString($"{Color} {Text}");
    }

    static Sticky IParser<Sticky>.Parse(in PacketReader p) => new(p);
}
