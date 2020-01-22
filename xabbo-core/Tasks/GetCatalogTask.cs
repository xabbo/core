using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class GetCatalogTask : InterceptorTask<Catalog>
    {
        private readonly string mode;

        public GetCatalogTask(IInterceptor interceptor, string mode)
            : base(interceptor)
        {
            this.mode = mode;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestCatalogMode, mode);

        [InterceptIn("CatalogPagesList")]
        private void OnCatalog(InterceptEventArgs e)
        {
            try
            {
                var catalog = Catalog.Parse(e.Packet);
                if (catalog.Mode == mode)
                {
                    e.Block();
                    SetResult(catalog);
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
