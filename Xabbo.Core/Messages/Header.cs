using System;

namespace Xabbo.Core.Messages
{
    public class Header : Identifier
    {
        public static readonly Header Unknown = new Header(Destination.Unknown, -1);

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
                Equals(other);
        }

        public bool Equals(Header other)
        {
            if (other is null) return false;

            return
                Value == other.Value &&
                (Destination == Destination.Unknown ||
                other.Destination == Destination.Unknown ||
                Destination == other.Destination) &&
                (!HasName || !other.HasName || string.Equals(Name, other.Name));
        }

        public override string ToString() => HasName ? $"{Name} ({Value})" : Value.ToString();

        public static implicit operator short(Header header) => header?.Value ?? -1;
        public static implicit operator Header(short value) => new Header(value);

        public static bool operator ==(Header a, Header b) => a.Equals(b);
        public static bool operator !=(Header a, Header b) => !(a == b);
    }
}
