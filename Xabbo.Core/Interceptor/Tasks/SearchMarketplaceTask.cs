using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    public class SearchMarketplaceTask : InterceptorTask<IEnumerable<IMarketplaceOffer>>
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

        protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(
            Out.MarketplaceSearchOffers,
            _from ?? -1, _to ?? -1,
            _searchText ?? string.Empty,
            (int)_sort
        );

        [InterceptIn(nameof(Incoming.MarketplaceOpenOfferList))]
        protected void HandleMarketplaceOpenOfferList(InterceptArgs e)
        {
            e.Block();

            List<IMarketplaceOffer> offers = new();
            short n = e.Packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
                offers.Add(MarketplaceOffer.Parse(e.Packet));

            SetResult(offers);
        }
    }
}
