using System;

namespace Xabbo.Core;

public interface IChatSettings
{
    ChatFlow Flow { get; }
    ChatBubbleWidth BubbleWidth { get; }
    ChatScrollSpeed ScrollSpeed { get; }
    int TalkHearingDistance { get; }
    ChatFloodProtection FloodProtection { get; }
}
