using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class Sticky : IComposer, IParser<Sticky>
{
    public static readonly StickyColors Colors = new();

    public Id Id { get; set; }
    public string Color { get; set; }
    public string Text { get; set; }

    public Sticky()
    {
        Color = string.Empty;
        Text = string.Empty;
    }

    private Sticky(in PacketReader p)
    {
        Id = p.Read<Id>();
        string text = p.Read<string>();
        int spaceIndex = text.IndexOf(' ');
        Color = text[0..6];
        if (spaceIndex == 6)
        {
            Text = text[7..];
        }
        else
        {
            Text = string.Empty;
        }
    }

    public static Sticky Parse(in PacketReader packet) => new(packet);

    public void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write($"{Color} {Text}");
    }
}
