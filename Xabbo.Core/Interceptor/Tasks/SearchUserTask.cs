using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    public class SearchUserTask : InterceptorTask<UserSearchResults>
    {
        private readonly string searchName;

        public SearchUserTask(IInterceptor interceptor, string searchName)
            : base(interceptor)
        {
            this.searchName = searchName;
        }

        protected override Task OnExecuteAsync() => SendAsync(Out.HabboSearch, searchName);

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
