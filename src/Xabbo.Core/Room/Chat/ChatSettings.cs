using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IChatSettings"/>.
public class ChatSettings : IChatSettings, IParserComposer<ChatSettings>
{
    public ChatFlow Flow { get; set; }
    public ChatBubbleWidth BubbleWidth { get; set; }
    public ChatScrollSpeed ScrollSpeed { get; set; }
    public int TalkHearingDistance { get; set; }
    public ChatFloodProtection FloodProtection { get; set; }

    public ChatSettings()
    {
        Flow = ChatFlow.LineByLine;
        BubbleWidth = ChatBubbleWidth.Normal;
        ScrollSpeed = ChatScrollSpeed.Normal;
        TalkHearingDistance = 14;
        FloodProtection = ChatFloodProtection.Standard;
    }

    internal ChatSettings(in PacketReader p)
    {
        Flow = (ChatFlow)p.ReadInt();
        BubbleWidth = (ChatBubbleWidth)p.ReadInt();
        ScrollSpeed = (ChatScrollSpeed)p.ReadInt();
        TalkHearingDistance = p.ReadInt();
        FloodProtection = (ChatFloodProtection)p.ReadInt();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt((int)Flow);
        p.WriteInt((int)BubbleWidth);
        p.WriteInt((int)ScrollSpeed);
        p.WriteInt(TalkHearingDistance);
        p.WriteInt((int)FloodProtection);
    }

    static ChatSettings IParser<ChatSettings>.Parse(in PacketReader p) => new(in p);
}
