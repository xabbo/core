using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    internal class GetStickyDataTask : InterceptorTask<Sticky>
    {
        private readonly int stickyId;

        public GetStickyDataTask(IInterceptor interceptor, int stickyId)
            : base(interceptor)
        {
            this.stickyId = stickyId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.PostItRequestData, stickyId);

        [InterceptIn("PostItData")]
        private void HandlePostItData(InterceptEventArgs e)
        {
            try
            {
                var sticky = Sticky.Parse(e.Packet);
                if (sticky.Id == stickyId)
                {
                    e.Block();
                    SetResult(sticky);
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
