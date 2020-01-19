using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class GroupMembers : List<GroupMember>
    {
        public static GroupMembers Parse(Packet packet) => new GroupMembers(packet);

        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int HomeRoomId { get; set; }
        public string BadgeCode { get; set; }
        public int TotalMatches { get; set; }
        public bool UnknownBoolA { get; set; }
        public int ResultsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public GroupMemberSearchType SearchType { get; set; }
        public string Filter { get; set; }

        public GroupMembers() { }

        protected GroupMembers(Packet packet)
        {
            GroupId = packet.ReadInteger();
            GroupName = packet.ReadString();
            HomeRoomId = packet.ReadInteger();
            BadgeCode = packet.ReadString();
            TotalMatches = packet.ReadInteger();
            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                Add(GroupMember.Parse(packet));
            UnknownBoolA = packet.ReadBoolean();
            ResultsPerPage = packet.ReadInteger();
            CurrentPage = packet.ReadInteger();
            SearchType = (GroupMemberSearchType)packet.ReadInteger();
            Filter = packet.ReadString();
        }
    }
}
