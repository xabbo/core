using System;

namespace Xabbo.Core.Messages
{
    public class Identifier
    {
        public Destination Destination { get; }
        public bool IsOutgoing => Destination == Destination.Server;
        public bool IsIncoming => Destination == Destination.Client;
        public string Name { get; }

        public Identifier(Destination destination, string name)
        {
            Destination = destination;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public Identifier WithDestination(Destination destination) => new Identifier(destination, Name);

        public override int GetHashCode()
        {
            int hashCode = Name.GetHashCode();
            return IsOutgoing ? hashCode : ~hashCode;
        }

        public override bool Equals(object? obj)
        {
            return
                obj is Identifier other &&
                Equals(other);
        }

        public bool Equals(Identifier other)
        {
            return
                Destination.Equals(other.Destination) &&
                string.Equals(Name, other.Name);
        }

        public override string ToString() => Name;

        public static bool operator ==(Identifier a, Identifier b)
        {
            if (a is null)
                return b is null;
            return a.Equals(b);
        }

        public static bool operator !=(Identifier a, Identifier b) => !(a == b);

        public static implicit operator Identifier(string name) => new Identifier(Destination.Unknown, name);
    }
}
