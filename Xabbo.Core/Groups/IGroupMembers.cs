using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public interface IGroupMembers : IReadOnlyList<IGroupMember>
    {
        long GroupId { get;}
        string GroupName { get; }
        long HomeRoomId { get; }
        string BadgeCode { get; }
        int TotalMatches { get; }
        int ResultsPerPage { get; }
        int CurrentPage { get; }
        GroupMemberSearchType SearchType { get; }
        string Filter { get; }
    }
}
