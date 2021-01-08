using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class GroupMembers : List<GroupMember>, IGroupMembers
    {
        public static GroupMembers Parse(IReadOnlyPacket packet) => new GroupMembers(packet);

        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int HomeRoomId { get; set; }
        public string BadgeCode { get; set; }
        public int TotalMatches { get; set; }
        public bool Bool1 { get; set; }
        public int ResultsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public GroupMemberSearchType SearchType { get; set; }
        public string Filter { get; set; }

        IGroupMember IReadOnlyList<IGroupMember>.this[int index] => this[index];
        IEnumerator<IGroupMember> IEnumerable<IGroupMember>.GetEnumerator() => GetEnumerator();

        public GroupMembers() { }

        protected GroupMembers(IReadOnlyPacket packet)
        {
            GroupId = packet.ReadInt();
            GroupName = packet.ReadString();
            HomeRoomId = packet.ReadInt();
            BadgeCode = packet.ReadString();
            TotalMatches = packet.ReadInt();
            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                Add(GroupMember.Parse(packet));
            Bool1 = packet.ReadBool();
            ResultsPerPage = packet.ReadInt();
            CurrentPage = packet.ReadInt();
            SearchType = (GroupMemberSearchType)packet.ReadInt();
            Filter = packet.ReadString();
        }
    }
}
