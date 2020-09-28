using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xabbo.Core.Messages
{
    public class Identifiers : HashSet<Identifier>
    {
        public IEnumerable<Identifier> Unknown => this.Where(id => id.Destination == Destination.Unknown);
        public IEnumerable<Identifier> Incoming => this.Where(id => id.Destination == Destination.Client);
        public IEnumerable<Identifier> Outgoing => this.Where(id => id.Destination == Destination.Server);

        public Identifiers() { }

        public Identifiers(IEnumerable<Identifier> identifiers)
            : base(identifiers)
        { }

        public Identifiers(string[] incoming = null, string[] outgoing = null)
        {
            if (incoming != null)
            {
                foreach (string identifier in incoming)
                    Add(new Identifier(Destination.Client, identifier));
            }

            if (outgoing != null)
            {
                foreach (string identifier in outgoing)
                    Add(new Identifier(Destination.Server, identifier));
            }
        }

        public void Add(Destination destination, string name) => Add(new Identifier(destination, name));
        public void Add(Destination destination, params string[] names)
        {
            foreach (string name in names)
                Add(destination, name);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (Unknown.Any())
                sb.Append(string.Join(", ", Unknown.Select(x => x.Name)));

            if (Incoming.Any())
            {
                if (sb.Length > 0) sb.Append("; ");
                sb.Append("Incoming: ");
                sb.Append(string.Join(", ", Incoming.Select(x => x.Name)));
            }

            if (Outgoing.Any())
            {
                if (sb.Length > 0) sb.Append("; ");
                sb.Append("Outgoing: ");
                sb.Append(string.Join(", ", Outgoing.Select(x => x.Name)));
            }

            return sb.ToString();
        }
    }
}
