using System;

using Xabbo.Common;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public class Sticky
    {
        public static readonly StickyColors Colors = new StickyColors();

        public long Id { get; set; }
        public string Color { get; set; }
        public string Text { get; set; }

        public Sticky()
        {
            Color = string.Empty;
            Text = string.Empty;
        }

        protected Sticky(IReadOnlyPacket packet)
        {
            Id = packet.Protocol switch
            {
                ClientType.Flash => long.Parse(packet.ReadString()),
                ClientType.Unity => packet.ReadLong(),
                _ => throw new Exception("Unknown client protocol.")
            };
            string text = packet.ReadString();
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

        public static Sticky Parse(IReadOnlyPacket packet) => new Sticky(packet);
    }
}
