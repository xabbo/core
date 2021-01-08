using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Tasks
{
    // @Update [RequiredOut(nameof(Outgoing.SearchUser))]
    public class SearchUserTask : InterceptorTask<UserSearchResults>
    {
        private readonly string searchName;

        public SearchUserTask(IInterceptor interceptor, string searchName)
            : base(interceptor)
        {
            this.searchName = searchName;
        }

        protected override Task OnExecuteAsync() => throw new NotImplementedException(); // @Update SendAsync(Out.SearchUser, searchName);

        // @Update [InterceptIn(nameof(Incoming.UserSearchResult))]
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
