using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

/// <remarks>
/// Sent when searching the list of a group's members.
/// <para/>
/// Request for <see cref="GroupMembersMsg"/>. Returns a <see cref="GroupMembers"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.GetGuildMembers"/></item>
/// </list>
/// </remarks>
/// <param name="Id">The ID of the group.</param>
/// <param name="Page">The page number of the group's member list to retrieve.</param>
/// <param name="Filter">The search text used to filter members by name.</param>
/// <param name="SearchType">The type of the member search.</param>
public sealed record SearchGroupMembersMsg(
    Id Id, int Page = 0, string Filter = "", GroupMemberSearchType SearchType = GroupMemberSearchType.Members
)
    : IRequestMessage<SearchGroupMembersMsg, GroupMembersMsg, GroupMembers>
{
    static ClientType IMessage<SearchGroupMembersMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<SearchGroupMembersMsg>.Identifier => Out.GetGuildMembers;

    bool IRequestFor<GroupMembersMsg>.MatchResponse(GroupMembersMsg msg) =>
        msg.Members.GroupId == Id &&
        msg.Members.PageIndex == Page &&
        msg.Members.Filter == Filter &&
        msg.Members.SearchType == SearchType;

    GroupMembers IResponseData<GroupMembersMsg, GroupMembers>.GetData(GroupMembersMsg msg) => msg.Members;

    static SearchGroupMembersMsg IParser<SearchGroupMembersMsg>.Parse(in PacketReader p) => new(
        Id: p.ReadId(),
        Page: p.ReadInt(),
        Filter: p.ReadString(),
        SearchType: (GroupMemberSearchType)p.ReadInt()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteInt(Page);
        p.WriteString(Filter);
        p.WriteInt((int)SearchType);
    }
}
