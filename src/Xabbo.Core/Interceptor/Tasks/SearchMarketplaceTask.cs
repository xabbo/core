using System.Collections.Generic;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

[Intercepts]
public partial class SearchMarketplaceTask : InterceptorTask<IEnumerable<IMarketplaceOffer>>
{
    private readonly string? _searchText;
    private readonly int? _from, _to;
    private readonly MarketplaceSortOrder _sort;

    public SearchMarketplaceTask(IInterceptor interceptor,
        string? searchText = null,
        int? from = null, int? to = null,
        MarketplaceSortOrder sort = MarketplaceSortOrder.HighestPrice)
        : base(interceptor)
    {
        _searchText = searchText;
        _from = from;
        _to = to;
        _sort = sort;
    }

    protected override void OnExecute() => Interceptor.Send(
        Out.GetMarketplaceOffers,
        _from ?? -1, _to ?? -1,
        _searchText ?? string.Empty,
        (int)_sort
    );

    [InterceptIn(nameof(In.MarketPlaceOffers))]
    protected void HandleMarketPlaceOffers(Intercept e)
    {
        e.Block();

        List<IMarketplaceOffer> offers = [];
        int n = e.Packet.Read<Length>();
        for (int i = 0; i < n; i++)
            offers.Add(e.Packet.Parse<MarketplaceOffer>());

        SetResult(offers);
    }
}
