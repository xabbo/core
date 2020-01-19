using System;
using System.Reflection;

namespace Xabbo.Core.Messages
{
    public abstract class ListenerCallback
    {
        private volatile bool isUnsubscribed = false;

        public short Header { get; }
        public object Target { get; }
        public MethodInfo Method { get; }
        public object[] Tags { get; }
        public Delegate Delegate { get; }
        public bool IsUnsubscribed => isUnsubscribed;

        public ListenerCallback(short header, object target, MethodInfo method, object[] tags, Delegate @delegate)
        {
            Header = header;
            Target = target;
            Method = method;
            Tags = tags;
            Delegate = @delegate;
        }

        public void Unsubscribe()
        {
            isUnsubscribed = true;
        }
    }
}
