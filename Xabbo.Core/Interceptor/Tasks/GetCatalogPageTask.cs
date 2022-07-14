using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetCatalogPageTask : InterceptorTask<ICatalogPage>
{
    private readonly int _pageId;
    private readonly string _catalogType;

    public GetCatalogPageTask(IInterceptor interceptor, int pageId, string catalogType)
        : base(interceptor)
    {
        _pageId = pageId;
        _catalogType = catalogType;
    }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.GetCatalogPage, _pageId, -1, _catalogType);

    [InterceptIn(nameof(Incoming.CatalogPage))]
    protected void HandleCatalogPage(InterceptArgs e)
    {
        try
        {
            var catalogPage = CatalogPage.Parse(e.Packet);
            if (catalogPage.Id == _pageId &&
                catalogPage.CatalogType == _catalogType)
            {
                if (SetResult(catalogPage))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
