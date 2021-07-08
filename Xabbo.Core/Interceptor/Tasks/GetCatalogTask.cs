using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    public class GetCatalogTask : InterceptorTask<ICatalog>
    {
        private readonly string mode;

        public GetCatalogTask(IInterceptor interceptor, string mode)
            : base(interceptor)
        {
            this.mode = mode;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.GetCatalogIndex, mode);

        [InterceptIn(nameof(Incoming.CatalogIndex))]
        protected void HandleCatalogIndex(InterceptArgs e)
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
