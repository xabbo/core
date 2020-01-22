using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    internal class GetCatalogPageTask : InterceptorTask<CatalogPage>
    {
        private readonly int pageId;
        private readonly string mode;

        public GetCatalogPageTask(IInterceptor interceptor, int pageId, string mode)
            : base(interceptor)
        {
            this.pageId = pageId;
            this.mode = mode;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestCatalogPage, pageId, mode);

        [InterceptIn("CatalogPage")]
        private void OnCatalogPage(InterceptEventArgs e)
        {
            try
            {
                var catalogPage = CatalogPage.Parse(e.Packet);
                if (catalogPage.Id == pageId &&
                    catalogPage.Mode == mode)
                {
                    e.Block();
                    SetResult(catalogPage);
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
