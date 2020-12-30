using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    // @Update [RequiredOut(nameof(Outgoing.RequestCatalogMode))]
    public class GetCatalogTask : InterceptorTask<ICatalog>
    {
        private readonly string mode;

        public GetCatalogTask(IInterceptor interceptor, string mode)
            : base(interceptor)
        {
            this.mode = mode;
        }

        protected override Task OnExecuteAsync() => throw new NotImplementedException(); // @Update  SendAsync(Out.RequestCatalogMode, mode);

        // @Update [InterceptIn(nameof(Incoming.CatalogPagesList))]
        protected void OnCatalogPagesList(InterceptArgs e)
        {
            try
            {
                var catalog = Catalog.Parse(e.Packet);
                if (catalog.Mode == mode)
                {
                    if (SetResult(catalog))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
