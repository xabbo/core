using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

public class GetGroupMembersTask : InterceptorTask<IGroupMembers>
{
    private readonly long _groupId;
    private readonly int _page;
    private readonly string _filter;
    private readonly GroupMemberSearchType _searchType;

    public GetGroupMembersTask(IInterceptor interceptor, long groupId, int page, string filter, GroupMemberSearchType searchType)
        : base(interceptor)
    {
        _groupId = groupId;
        _page = page;
        _filter = filter;
        _searchType = searchType;
    }

    /// <inheritdoc/>
    protected override ValueTask OnExecuteAsync() => Interceptor.SendAsync(Out.GetGuildMembers, (LegacyLong)_groupId, _page, _filter, (int)_searchType);

    [InterceptIn(nameof(Incoming.GuildMembers))] 
    protected void OnGuildMembers(InterceptArgs e)
    {
        try
        {
            var groupMembers = GroupMembers.Parse(e.Packet);
            if (groupMembers.GroupId == _groupId &&
                groupMembers.PageIndex == _page &&
                groupMembers.Filter == _filter &&
                groupMembers.SearchType == _searchType)
            {
                if (SetResult(groupMembers))
                    e.Block();
            }
        }
        catch (Exception ex) { SetException(ex); }
    }
}
