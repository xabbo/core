using System;
using System.Linq;
using System.Text;

namespace Xabbo.Core.Messages
{
    public class UnresolvedIdentifiersException : Exception
    {
        public Identifiers Identifiers { get; }

        public UnresolvedIdentifiersException(Identifiers identifiers, string message)
            : base(ConstructMessage(identifiers, message))
        {
            Identifiers = identifiers;
        }

        private static string ConstructMessage(Identifiers identifiers, string message)
        {
            var sb = new StringBuilder();
            sb.Append(message);
            sb.Append(": Unresolved identifiers (");

            if (identifiers.Incoming.Any())
            {
                sb.Append("Incoming: ");
                sb.Append(string.Join(", ", identifiers.Incoming));
            }

            if (identifiers.Outgoing.Any())
            {
                if (identifiers.Incoming.Any())
                    sb.Append("; ");
                sb.Append("Outgoing: ");
                sb.Append(string.Join(", ", identifiers.Outgoing));
            }

            sb.Append(")");

            return sb.ToString();
        }
    }
}
