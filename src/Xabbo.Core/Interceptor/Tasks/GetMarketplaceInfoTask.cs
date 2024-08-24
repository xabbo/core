using System;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

[Intercepts]
public sealed partial class GetMarketplaceInfoTask(IInterceptor interceptor, ItemType type, int kind)
    : InterceptorTask<IMarketplaceItemInfo>(interceptor)
{
    private readonly ItemType _type = type;
    private readonly int _kind = kind;

    protected override void OnExecute() => Interceptor.Send(
        Out.GetMarketplaceItemStats,
        _type switch
        {
            ItemType.Floor => 1,
            ItemType.Wall => 2,
            _ => throw new InvalidOperationException($"Invalid item type: {_type}.")
        },
        _kind
    );

    [InterceptIn(nameof(In.MarketplaceItemStats))]
    private void HandleMarketplaceItemStats(Intercept e)
    {
        MarketplaceItemInfo info = e.Packet.Parse<MarketplaceItemInfo>();

        if (info.Type == _type && info.Kind == _kind)
        {
            e.Block();
            SetResult(info);
        }
    }
}
