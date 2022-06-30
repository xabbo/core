using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class ChatSettings : IChatSettings, IComposable
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

        internal ChatSettings(IReadOnlyPacket packet)
        {
            Flow = (ChatFlow)packet.ReadInt();
            BubbleWidth = (ChatBubbleWidth)packet.ReadInt();
            ScrollSpeed = (ChatScrollSpeed)packet.ReadInt();
            TalkHearingDistance = packet.ReadInt();
            FloodProtection = (ChatFloodProtection)packet.ReadInt();
        }

        public void Compose(IPacket packet) => packet
            .WriteInt((int)Flow)
            .WriteInt((int)BubbleWidth)
            .WriteInt((int)ScrollSpeed)
            .WriteInt(TalkHearingDistance)
            .WriteInt((int)FloodProtection);

        public static ChatSettings Parse(IReadOnlyPacket packet)
        {
            return new ChatSettings(packet);
        }
    }
}
