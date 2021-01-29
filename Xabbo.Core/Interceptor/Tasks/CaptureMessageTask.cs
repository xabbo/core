using System;
using System.Linq;
using System.Threading.Tasks;

using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Tasks
{
    public class CaptureMessageTask : InterceptorTask<IReadOnlyPacket>
    {
        private readonly bool blockPacket;
        private readonly Destination destination;
        private readonly Header[] targetHeaders;

        public CaptureMessageTask(IInterceptor interceptor,
            bool blockPacket, Destination destination, params Header[] targetHeaders)
            : base(interceptor)
        {
            if (targetHeaders.Any(header => header < 0))
                throw new Exception("Invalid target header specified.");

            this.blockPacket = blockPacket;
            this.destination = destination;
            this.targetHeaders = targetHeaders;
        }

        public CaptureMessageTask(IInterceptor interceptor, Destination destination, params Header[] targetHeaders)
            : this(interceptor, false, destination, targetHeaders)
        { }

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

        private void OnIntercept(InterceptArgs e)
        {
            try
            {
                if (SetResult(new Packet(e.Packet)))
                {
                    if (blockPacket)
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}