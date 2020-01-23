using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core.Messages
{
    internal abstract class ReceiveCallback<TSender> : ListenerCallback
    {
        protected ReceiveCallback(short header, object target, object[] tags, Delegate @delegate)
            : base(header, target, @delegate.Method, tags, @delegate)
        { }

        public void Invoke(TSender sender, Packet packet)
        {
            if (isUnsubscribed) return;
            OnInvoked(sender, packet);
        }

        protected abstract void OnInvoked(TSender sender, Packet packet);
    }

    internal class OpenReceiverCallback<TSender> : ReceiveCallback<TSender>
    {
        private readonly Action<object, TSender, Packet> callback;

        public OpenReceiverCallback(short header, object target, object[] tags, Action<object, TSender, Packet> callback)
            : base(header, target, tags, callback)
        {
            this.callback = callback;
        }

        protected override void OnInvoked(TSender sender, Packet packet)
        {
            callback(Target, sender, packet);
        }
    }

    internal sealed class SenderReceiverCallback<TSender> : ReceiveCallback<TSender>
    {
        private Action<TSender, Packet> call;

        public SenderReceiverCallback(short header, object target, object[] tags, Delegate @delegate)
            : base(header, target, tags, @delegate)
        {
            call = @delegate as Action<TSender, Packet>;
            if (call == null)
                throw new Exception($"Invalid delegate type {@delegate.GetType().Name} for {GetType().Name}");
        }

        protected override void OnInvoked(TSender sender, Packet packet)
        {
            call(sender, packet);
        }
    }
}
