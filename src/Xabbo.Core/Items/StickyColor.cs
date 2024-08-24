using System;
using Xabbo.Messages;

namespace Xabbo.Core;

public class StickyColor : IComposer
{
    public string Name { get; }
    public string Value { get; }

    public StickyColor(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public static implicit operator string(StickyColor color) => color.Value;

    public void Compose(in PacketWriter packet) => packet.Write(Value);
}
