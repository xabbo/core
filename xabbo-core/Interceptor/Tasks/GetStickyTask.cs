using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut(nameof(Outgoing.PostItRequestData))]
    public class GetStickyTask : InterceptorTask<Sticky>
    {
        private readonly int stickyId;

        public GetStickyTask(IInterceptor interceptor, int stickyId)
            : base(interceptor)
        {
            this.stickyId = stickyId;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.PostItRequestData, stickyId);

        [InterceptIn(nameof(Incoming.PostItData))]
        protected void OnPostItData(InterceptArgs e)
        {
            try
            {
                var sticky = Sticky.Parse(e.Packet);
                if (sticky.Id == stickyId)
                {
                    if (SetResult(sticky))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
