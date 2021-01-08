using System;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core.Messages
{
    public abstract class IdentifiersAttribute : Attribute
    {
        public IReadOnlyList<Identifier> Identifiers { get; private set; }

        internal IdentifiersAttribute(Destination destination, params string[] identifiers)
        {
            if (identifiers == null || identifiers.Length == 0)
                throw new ArgumentException("At least one identifier must be defined");

            Identifiers = identifiers.Select(name => new Identifier(destination, name)).Distinct().ToList().AsReadOnly();
        }
    }
}
