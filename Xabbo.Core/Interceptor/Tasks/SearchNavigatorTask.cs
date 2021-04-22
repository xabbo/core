using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    public class SearchNavigatorTask : InterceptorTask<NavigatorSearchResults>
    {
        private readonly string category;
        private readonly string filter;

        public SearchNavigatorTask(IInterceptor interceptor, string category, string filter)
            : base(interceptor)
        {
            this.category = category;
            this.filter = filter;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.Navigator2Search, category, filter);

        [InterceptIn(nameof(Incoming.Navigator2SearchResultBlocks))]
        protected void OnNavigatorSearchResults(InterceptArgs e)
        {
            try
            {
                var results = NavigatorSearchResults.Parse(e.Packet);
                if (results.Category == category &&
                    results.Filter == filter)
                {
                    if (SetResult(results))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
