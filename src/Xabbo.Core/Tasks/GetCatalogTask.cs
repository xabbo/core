using System;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

[Intercept]
public sealed partial class GetCatalogTask(IInterceptor interceptor, string type) : InterceptorTask<ICatalog>(interceptor)
{
    private readonly string _type = type;

    protected override ClientType SupportedClients => ~ClientType.Shockwave;

    protected override void OnExecute() => Interceptor.Send(Out.GetCatalogIndex, _type);

    [InterceptIn(nameof(In.CatalogIndex))]
    void HandleCatalogIndex(Intercept e)
    {
        try
        {
            var catalog = e.Packet.Read<Catalog>();
            if (catalog.Type == _type)
            {
                if (SetResult(catalog))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
