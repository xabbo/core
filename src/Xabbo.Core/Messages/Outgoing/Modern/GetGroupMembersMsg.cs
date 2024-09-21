using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Messages.Outgoing.Modern;

public sealed record GetGroupMembersMsg(
    Id Id, int Page = 0, string Filter = "", GroupMemberSearchType SearchType = GroupMemberSearchType.Members
)
    : IRequestMessage<GetGroupMembersMsg, GroupMembersMsg, GroupMembers>
{
    static Identifier IMessage<GetGroupMembersMsg>.Identifier => Out.GetGuildMembers;

    bool IRequestFor<GroupMembersMsg>.MatchResponse(GroupMembersMsg msg) =>
        msg.Members.GroupId == Id &&
        msg.Members.PageIndex == Page &&
        msg.Members.Filter == Filter &&
        msg.Members.SearchType == SearchType;

    GroupMembers IResponseData<GroupMembersMsg, GroupMembers>.GetData(GroupMembersMsg msg) => msg.Members;

    static GetGroupMembersMsg IParser<GetGroupMembersMsg>.Parse(in PacketReader p) => new(
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
