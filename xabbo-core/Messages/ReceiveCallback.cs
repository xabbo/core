using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core.Messages
{
    internal abstract class ReceiveCallback : ListenerCallback
    {
        protected ReceiveCallback(short header, object target, object[] tags, Delegate @delegate)
            : base(header, target, @delegate.Method, tags, @delegate)
        { }

        public void Invoke(object sender, Packet packet)
        {
            if (isUnsubscribed) return;
            OnInvoked(sender, packet);
        }

        protected abstract void OnInvoked(object sender, Packet packet);
    }

    internal class OpenReceiverCallback : ReceiveCallback
    {
        private readonly Action<object, object, Packet> callback;

        public OpenReceiverCallback(short header, object target, object[] tags, Action<object, object, Packet> callback)
            : base(header, target, tags, callback)
        {
            this.callback = callback;
        }

        protected override void OnInvoked(object sender, Packet packet)
        {
            callback(Target, sender, packet);
        }
    }

    internal sealed class SenderReceiverCallback : ReceiveCallback
    {
        private Action<object, Packet> call;

        public SenderReceiverCallback(short header, object target, object[] tags, Delegate @delegate)
            : base(header, target, tags, @delegate)
        {
            call = @delegate as Action<object, Packet>;
            if (call == null)
                throw new Exception($"Invalid delegate type {@delegate.GetType().Name} for {GetType().Name}");
        }

        protected override void OnInvoked(object sender, Packet packet)
        {
            call(sender, packet);
        }
    }
}
