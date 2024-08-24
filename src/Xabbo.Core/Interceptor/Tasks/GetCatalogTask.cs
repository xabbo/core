using System;
using System.Threading.Tasks;

using Xabbo.Messages.Flash;
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

    protected override void OnExecute() => Interceptor.Send(Out.GetCatalogIndex, _type);

    [InterceptIn(nameof(In.CatalogIndex))]
    protected void HandleCatalogIndex(Intercept e)
    {
        try
        {
            var catalog = e.Packet.Parse<Catalog>();
            if (catalog.Type == _type)
            {
                if (SetResult(catalog))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
