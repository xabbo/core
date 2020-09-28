using System;
using System.CodeDom;
using System.Runtime.CompilerServices;

namespace Xabbo.Core.Messages
{
    public class Header : Identifier
    {
        public static readonly Header Unknown = -1;

        public bool HasName { get; }
        public short Value { get; }

        public Header(Destination destination, string name, short value)
            : base(destination, name)
        {
            HasName = true;
            Value = value;
        }

        public Header(Destination destination, short value)
            : base(destination, value.ToString())
        {
            Value = value;
        }

        public Header(short value)
            : base(Destination.Unknown, value.ToString())
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() + (Value * 43003);
        }

        public override bool Equals(object obj)
        {
            return
                obj is Header other &&
                this == other;
        }

        public override string ToString() => HasName ? $"{Name} ({Value})" : Value.ToString();

        public static implicit operator short(Header header) => header?.Value ?? -1;
        public static implicit operator Header(short value) => new Header(value);

        public static bool operator ==(Header a, Header b)
        {
            return
                a.Value == b.Value &&
                (a.Destination == Destination.Unknown ||
                b.Destination == Destination.Unknown ||
                a.Destination == b.Destination) &&
                (!a.HasName || !b.HasName || string.Equals(a.Name, b.Name));
        }
        public static bool operator !=(Header a, Header b) => !(a == b);
    }
}
