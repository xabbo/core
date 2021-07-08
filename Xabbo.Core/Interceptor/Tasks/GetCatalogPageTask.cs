using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    public class GetCatalogPageTask : InterceptorTask<ICatalogPage>
    {
        private readonly int _pageId;
        private readonly string _mode;

        public GetCatalogPageTask(IInterceptor interceptor, int pageId, string mode)
            : base(interceptor)
        {
            _pageId = pageId;
            _mode = mode;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.GetCatalogPage, _pageId, -1, _mode);

        [InterceptIn(nameof(Incoming.CatalogPage))]
        protected void HandleCatalogPage(InterceptArgs e)
        {
            try
            {
                var catalogPage = CatalogPage.Parse(e.Packet);
                if (catalogPage.Id == _pageId &&
                    catalogPage.Mode == _mode)
                {
                    if (SetResult(catalogPage))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
