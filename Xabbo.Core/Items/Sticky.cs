using System;
using Xabbo.Core.Protocol;

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
            Id = packet.ReadLong();
            string text = packet.ReadString();
            int spaceIndex = text.IndexOf(' ');
            if (spaceIndex != 6 && text.Length != 6)
               throw new FormatException($"Sticky data is of an invalid format: '{text}'");
            Color = text[0..6];
            Text = text[7..];
        }

        public static Sticky Parse(IReadOnlyPacket packet) => new Sticky(packet);
    }
}
