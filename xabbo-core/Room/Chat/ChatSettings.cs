using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class ChatSettings : IWritable
    {
        public static ChatSettings Parse(Packet packet) => new ChatSettings(packet);

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

        internal ChatSettings(Packet packet)
        {
            Flow = (ChatFlow)packet.ReadInteger();
            BubbleWidth = (ChatBubbleWidth)packet.ReadInteger();
            ScrollSpeed = (ChatScrollSpeed)packet.ReadInteger();
            TalkHearingDistance = packet.ReadInteger();
            FloodProtection = (ChatFloodProtection)packet.ReadInteger();
        }

        public void Write(Packet packet) => packet.WriteValues(
            (int)Flow,
            (int)BubbleWidth,
            (int)ScrollSpeed,
            TalkHearingDistance,
            (int)FloodProtection
        );
    }
}
