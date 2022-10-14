using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetCatalogTask : InterceptorTask<ICatalog>
{
    private readonly string _type;

    public GetCatalogTask(IInterceptor interceptor, string type)
        : base(interceptor)
    {
        _type = type;
    }

    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.GetCatalogIndex, _type);

    [InterceptIn(nameof(Incoming.CatalogIndex))]
    protected void HandleCatalogIndex(InterceptArgs e)
    {
        try
        {
            var catalog = Catalog.Parse(e.Packet);
            if (catalog.Type == _type)
            {
                if (SetResult(catalog))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
