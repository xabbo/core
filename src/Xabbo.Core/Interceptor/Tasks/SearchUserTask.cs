using System;

using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Tasks;

[Intercepts]
public partial class SearchUserTask(IInterceptor interceptor, string searchName)
    : InterceptorTask<UserSearchResults>(interceptor)
{
    private readonly string _searchName = searchName;

    protected override void OnExecute() => Interceptor.Send(Out.HabboSearch, _searchName);

    [InterceptIn(nameof(In.HabboSearchResult))]
    void OnUserSearchResult(Intercept e)
    {
        try
        {
            if (SetResult(e.Packet.Parse<UserSearchResults>()))
                e.Block();
        }
        catch (Exception ex) { SetException(ex); }
    }
}
