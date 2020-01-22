using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    [RequiredOut("RequestNewNavigatorRooms")]
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

        protected override Task OnExecuteAsync() => SendAsync(Out.RequestNewNavigatorRooms, category, filter);

        [InterceptIn("NewNavigatorSearchResults")]
        private void OnNavigatorSearchResults(InterceptEventArgs e)
        {
            try
            {
                var results = NavigatorSearchResults.Parse(e.Packet);
                if (results.Category == category &&
                    results.Filter == filter)
                {
                    e.Block();
                    SetResult(results);
                }
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
