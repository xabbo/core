using System.Collections.Generic;

using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Tasks;

[Intercept]
public sealed partial class SearchMarketplaceTask(
    IInterceptor interceptor,
    string? searchText = null,
    int? from = null, int? to = null,
    MarketplaceSortOrder sort = MarketplaceSortOrder.HighestPrice
)
    : InterceptorTask<ICollection<IMarketplaceOffer>>(interceptor)
{
    private readonly string? _searchText = searchText;
    private readonly int? _from = from, _to = to;
    private readonly MarketplaceSortOrder _sort = sort;

    protected override void OnExecute() => Interceptor.Send(
        Out.GetMarketplaceOffers,
        _from ?? -1, _to ?? -1,
        _searchText ?? "",
        (int)_sort
    );

    [InterceptIn(nameof(In.MarketPlaceOffers))]
    void HandleMarketPlaceOffers(Intercept e)
    {
        e.Block();
        SetResult(e.Packet.Read<MarketplaceOffer[]>());
    }
}
