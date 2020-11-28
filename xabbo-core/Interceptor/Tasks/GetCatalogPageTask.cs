using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    [RequiredOut(nameof(Outgoing.RequestCatalogPage))]
    public class GetCatalogPageTask : InterceptorTask<ICatalogPage>
    {
        private readonly int pageId;
        private readonly string mode;

        public GetCatalogPageTask(IInterceptor interceptor, int pageId, string mode)
            : base(interceptor)
        {
            this.pageId = pageId;
            this.mode = mode;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestCatalogPage, pageId, -1, mode);

        [InterceptIn(nameof(Incoming.CatalogPage))]
        protected void OnCatalogPage(InterceptArgs e)
        {
            try
            {
                var catalogPage = CatalogPage.Parse(e.Packet);
                if (catalogPage.Id == pageId &&
                    catalogPage.Mode == mode)
                {
                    if (SetResult(catalogPage))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
