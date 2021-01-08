using System;
using System.Collections.Generic;

namespace Xabbo.Core.Messages
{
    internal class ListenerAttachment
    {
        public IListener Listener { get; }
        public IReadOnlyCollection<ListenerCallback> Callbacks { get; }
        public IReadOnlyCollection<object> AttachedGroups { get; }
        public IReadOnlyCollection<object> FaultedGroups { get; }

        public ListenerAttachment(IListener listener, IEnumerable<ListenerCallback> callbacks,
            IEnumerable<object> attachedGroups, IEnumerable<object> faultedGroups)
        {
            Listener = listener;
            Callbacks = new List<ListenerCallback>(callbacks).AsReadOnly();
            AttachedGroups = new List<object>(attachedGroups).AsReadOnly();
            FaultedGroups = new List<object>(faultedGroups).AsReadOnly();
        }
    }
}
