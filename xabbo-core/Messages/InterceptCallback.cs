using System;
using System.Reflection;

namespace Xabbo.Core.Messages
{
    internal abstract class InterceptCallback : ListenerCallback
    {
        public Destination Destination { get; }
        public bool IsOutgoing => Destination == Destination.Server;
        public bool IsIncoming => Destination == Destination.Client;

        public InterceptCallback(Destination destination, short header, object target, MethodInfo method, object[] tags, Delegate @delegate)
            : base(header, target, method, tags, @delegate)
        {
            Destination = destination;
        }

        public abstract void Invoke(InterceptArgs e);
    }

    internal class OpenInterceptCallback : InterceptCallback
    {
        private readonly Action<object, InterceptArgs> callback;

        public OpenInterceptCallback(Destination destination, short header,
            object target, MethodInfo method, object[] tags,
            Action<object, InterceptArgs> callback)
            : base(destination, header, target, method, tags, callback)
        {
            this.callback = callback;
        }

        public override void Invoke(InterceptArgs e)
        {
            callback(Target, e);
        }
    }

    internal class ClosedInterceptCallback : InterceptCallback
    {
        private readonly Action<InterceptArgs> callback;

        public ClosedInterceptCallback(Destination destination, short header,
            object target, MethodInfo method, object[] tags,
            Action<InterceptArgs> callback)
            : base(destination, header, target, method, tags, callback)
        {
            this.callback = callback;
        }

        public override void Invoke(InterceptArgs e)
        {
            callback(e);
        }
    }
}
