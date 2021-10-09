using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks
{
    public class SearchUserTask : InterceptorTask<UserSearchResults>
    {
        private readonly string _searchName;

        public SearchUserTask(IInterceptor interceptor, string searchName)
            : base(interceptor)
        {
            this._searchName = searchName;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.HabboSearch, _searchName);

        [InterceptIn(nameof(Incoming.HabboSearchResult))]
        protected void OnUserSearchResult(InterceptArgs e)
        {
            try
            {
                if (SetResult(UserSearchResults.Parse(e.Packet)))
                    e.Block();
            }
            catch (Exception ex) { SetException(ex); }
        }
    }
}
