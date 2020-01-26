using System;
using System.Collections.Generic;
using System.Text;

namespace Xabbo.Core.Messages
{
    public class ListenerAttachFailedException : Exception
    {
        public IListener Listener { get; }
        public IReadOnlyCollection<object> FaultedGroups { get; }

        public ListenerAttachFailedException(IListener listener, params object[] faultedGroups)
            : base(BuildMessage(listener, faultedGroups))
        {
            Listener = listener;
            FaultedGroups = new List<object>(faultedGroups).AsReadOnly();
        }

        private static string BuildMessage(IListener listener, object[] faultedGroups)
        {
            var sb = new StringBuilder();
            sb.Append($"Failed to attach listener '{listener.GetType().FullName}'");

            if (faultedGroups.Length > 1
                || (faultedGroups.Length == 1 && !ReferenceEquals(faultedGroups[0], MessageGroups.Default)))
            {
                sb.Append($"; faulted groups: ");
                sb.Append(string.Join(", ", faultedGroups));
            }

            return sb.ToString();
        }
    }
}
