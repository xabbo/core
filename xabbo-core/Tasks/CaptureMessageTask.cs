using System;
using System.Linq;
using System.Threading.Tasks;

using Xabbo.Core.Protocol;
using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    internal class CaptureMessageTask : InterceptorTask<Packet>
    {
        private readonly Destination destination;
        private readonly short[] targetHeaders;

        public CaptureMessageTask(IInterceptor interceptor, Destination destination, params short[] targetHeaders)
            : base(interceptor)
        {
            if (targetHeaders.Any(header => header < 0))
                throw new Exception("Invalid message header");

            this.destination = destination;
            this.targetHeaders = targetHeaders;
        }

        protected override void Hook()
        {
            foreach (short header in targetHeaders)
                Dispatcher.AddIntercept(destination, header, OnIntercept);
        }

        protected override void Unhook()
        {
            foreach (short header in targetHeaders)
                Dispatcher.AddIntercept(destination, header, OnIntercept);
        }

        protected override Task OnExecuteAsync() => Task.CompletedTask;

        private void OnIntercept(InterceptEventArgs e)
        {
            try { SetResult(e.Packet.Clone()); }
            catch (Exception ex) { SetException(ex); }
        }
    }
}