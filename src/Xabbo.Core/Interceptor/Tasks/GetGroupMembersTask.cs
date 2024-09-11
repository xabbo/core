using System;

using Xabbo.Messages.Flash;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Tasks;

namespace Xabbo.Core.Tasks;

[Intercept]
public sealed partial class GetGroupMembersTask(
    IInterceptor interceptor, Id groupId, int page, string filter, GroupMemberSearchType searchType
)
    : InterceptorTask<IGroupMembers>(interceptor)
{
    private readonly Id _groupId = groupId;
    private readonly int _page = page;
    private readonly string _filter = filter;
    private readonly GroupMemberSearchType _searchType = searchType;


    /// <inheritdoc/>
    protected override void OnExecute() => Interceptor.Send(Out.GetGuildMembers, _groupId, _page, _filter, (int)_searchType);

    [InterceptIn(nameof(In.GuildMembers))]
    private void HandleGuildMembers(Intercept e)
    {
        try
        {
            var groupMembers = e.Packet.Read<GroupMembers>();
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
