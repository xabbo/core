using System.Collections.Generic;

namespace Xabbo.Core;

/// <summary>
/// Represents a list of group members.
/// </summary>
public interface IGroupMembers : IReadOnlyList<IGroupMember>
{
    Id GroupId { get; }
    string GroupName { get; }
    Id HomeRoomId { get; }
    string BadgeCode { get; }
    int TotalEntries { get; }
    bool IsAllowedToManage { get; }
    int PageSize { get; }
    int PageIndex { get; }
    GroupMemberSearchType SearchType { get; }
    string Filter { get; }
}
