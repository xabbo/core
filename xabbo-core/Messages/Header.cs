using System;

namespace Xabbo.Core.Messages
{
    public class Header : Identifier
    {
        public short Value { get; }

        public Header(Destination destination, string name, short value)
            : base(destination, name)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            return
                obj is Header other &&
                base.Equals(other) &&
                Value == other.Value;
        }

        public override string ToString() => Name ?? Value.ToString();

        public static implicit operator short(Header header) => header.Value;
        public static implicit operator Header(short value) => new Header(Destination.Unknown, value.ToString(), value);
    }
}
