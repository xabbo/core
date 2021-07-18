using System;
using System.Threading.Tasks;

using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Messages;

namespace Xabbo.Core.Tasks
{
    public class GetUserMarketplaceOffersTask : InterceptorTask<IUserMarketplaceOffers>
    {
        public GetUserMarketplaceOffersTask(IInterceptor interceptor)
            : base(interceptor)
        { }

        protected override Task OnExecuteAsync() => SendAsync(Out.MarketplaceListOwnOffers);

        [InterceptIn(nameof(Incoming.MarketplaceOwnOfferList))]
        protected void HandleMarketplaceOwnOfferList(InterceptArgs e)
        {
            e.Block();

            SetResult(UserMarketplaceOffers.Parse(e.Packet));
        }
    }
}
