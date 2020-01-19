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
    }
}
