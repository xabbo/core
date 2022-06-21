using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Interceptor.Attributes;

namespace Xabbo.Core.Tasks
{
    public class SearchNavigatorTask : InterceptorTask<NavigatorSearchResults>
    {
        private readonly string _category;
        private readonly string _filter;

        public SearchNavigatorTask(IInterceptor interceptor, string category, string filter)
            : base(interceptor)
        {
            _category = category;
            _filter = filter;
        }

        protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.Navigator2Search, _category, _filter);

        [InterceptIn(nameof(Incoming.Navigator2SearchResultBlocks))]
        protected void OnNavigatorSearchResults(InterceptArgs e)
        {
            try
            {
                var results = NavigatorSearchResults.Parse(e.Packet);
                if (results.Category == _category &&
                    results.Filter == _filter)
                {
                    if (SetResult(results))
                        e.Block();
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
