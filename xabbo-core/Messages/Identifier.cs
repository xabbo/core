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
            if (name == null)
                throw new ArgumentNullException("name");

            Destination = destination;
            Name = name;
        }

        public override int GetHashCode()
        {
            int hashCode = Name.GetHashCode();
            return IsOutgoing ? hashCode : ~hashCode;
        }

        public override bool Equals(object obj)
        {
            Identifier other = obj as Identifier;
            if (other == null) return false;
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
    }
}
