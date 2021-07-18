using System;
using System.Threading.Tasks;

using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Messages;

namespace Xabbo.Core.Tasks
{
    public class GetMarketplaceInfoTask : InterceptorTask<IMarketplaceItemInfo>
    {
        private readonly ItemType _type;
        private readonly int _kind;

        public GetMarketplaceInfoTask(IInterceptor interceptor, ItemType type, int kind)
            : base(interceptor)
        {
            _type = type;
            _kind = kind;
        }

        protected override Task OnExecuteAsync() => SendAsync(
            Out.MarketplaceGetItemStats,
            _type switch
            {
                ItemType.Floor => 1,
                ItemType.Wall => 2,
                _ => throw new InvalidOperationException($"Invalid item type: {_type}.")
            },
            _kind
        );

        [InterceptIn(nameof(Incoming.MarketplaceItemStats))]
        protected void HandleMarketplaceItemStats(InterceptArgs e)
        {
            MarketplaceItemInfo info = MarketplaceItemInfo.Parse(e.Packet);

            if (info.Type == _type && info.Kind == _kind)
            {
                e.Block();
                SetResult(info);
            }
        }
    }
}
