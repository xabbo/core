using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public class ChatSettings : IChatSettings, IComposer, IParser<ChatSettings>
{
    public ChatFlow Flow { get; set; }
    public ChatBubbleWidth BubbleWidth { get; set; }
    public ChatScrollSpeed ScrollSpeed { get;  set; }
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
        Flow = (ChatFlow)p.Read<int>();
        BubbleWidth = (ChatBubbleWidth)p.Read<int>();
        ScrollSpeed = (ChatScrollSpeed)p.Read<int>();
        TalkHearingDistance = p.Read<int>();
        FloodProtection = (ChatFloodProtection)p.Read<int>();
    }

    public void Compose(in PacketWriter p)
    {
        p.Write((int)Flow);
        p.Write((int)BubbleWidth);
        p.Write((int)ScrollSpeed);
        p.Write(TalkHearingDistance);
        p.Write((int)FloodProtection);
    }

    public static ChatSettings Parse(in PacketReader p) => new(in p);
}
