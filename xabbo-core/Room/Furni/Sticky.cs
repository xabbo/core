using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Sticky
    {
        public static readonly StickyColors Colors = new StickyColors();

        public int Id { get; set; }
        public string Color { get; set; }
        public string Text { get; set; }

        public Sticky() { }

        protected Sticky(Packet packet)
        {
            Id = int.Parse(packet.ReadString());
            string text = packet.ReadString();
            int spaceIndex = text.IndexOf(' ');
            if (spaceIndex != 6 && text.Length != 6)
               throw new FormatException($"Sticky data is of an invalid format: '{text}'");
            Color = text.Substring(0, 6);
            Text = text.Substring(7);
        }

        public static Sticky Parse(Packet packet) => new Sticky(packet);
    }
}
