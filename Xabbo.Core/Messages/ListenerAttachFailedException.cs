using System;
using System.Text;

namespace Xabbo.Core.Messages
{
    public class ListenerAttachFailedException : Exception
    {
        public IListener Listener { get; }
        public Identifiers UnresolvedIdentifiers { get; }

        public ListenerAttachFailedException(IListener listener, Identifiers unresolved)
            : base(BuildMessage(listener, unresolved))
        {
            Listener = listener;
            UnresolvedIdentifiers = unresolved;
        }

        private static string BuildMessage(IListener listener, Identifiers unresolved)
        {
            var sb = new StringBuilder();
            sb.Append($"Failed to attach listener '{listener.GetType().FullName}'.");

            if (unresolved.Count > 0)
            {
                sb.Append(" Unresolved identifiers - ");
                sb.Append(unresolved.ToString());
            }

            return sb.ToString();
        }
    }
}
