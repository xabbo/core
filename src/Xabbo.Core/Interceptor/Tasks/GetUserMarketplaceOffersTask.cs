using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetUserMarketplaceOffersTask : InterceptorTask<IUserMarketplaceOffers>
{
    public GetUserMarketplaceOffersTask(IInterceptor interceptor)
        : base(interceptor)
    { }

    protected override void OnExecute() => Interceptor.Send(Out.GetMarketplaceOwnOffers);

    [InterceptIn(nameof(In.MarketPlaceOwnOffers))]
    protected void HandleMarketplaceOwnOffers(Intercept e)
    {
        e.Block();

        SetResult(e.Packet.Read<UserMarketplaceOffers>());
    }
}
