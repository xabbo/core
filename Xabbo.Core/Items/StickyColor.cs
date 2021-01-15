using System;
using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class StickyColor : IPacketData
    {
        public string Name { get; }
        public string Value { get; }

        public StickyColor(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public static implicit operator string(StickyColor color) => color.Value;

        public void Write(IPacket packet) => packet.WriteString(Value);
    }
}
