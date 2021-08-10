using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class GroupMembers : List<GroupMember>, IGroupMembers
    {
        public static GroupMembers Parse(IReadOnlyPacket packet) => new(packet);

        public long GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public long HomeRoomId { get; set; }
        public string BadgeCode { get; set; } = string.Empty;
        public int TotalEntries { get; set; }
        public bool IsAllowedToManage { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public GroupMemberSearchType SearchType { get; set; }
        public string Filter { get; set; } = string.Empty;

        IGroupMember IReadOnlyList<IGroupMember>.this[int index] => this[index];
        IEnumerator<IGroupMember> IEnumerable<IGroupMember>.GetEnumerator() => GetEnumerator();

        public GroupMembers() { }

        protected GroupMembers(IReadOnlyPacket packet)
        {
            GroupId = packet.ReadLegacyLong();
            GroupName = packet.ReadString();
            HomeRoomId = packet.ReadLegacyLong();
            BadgeCode = packet.ReadString();
            TotalEntries = packet.ReadInt();
            short n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
                Add(GroupMember.Parse(packet));
            IsAllowedToManage = packet.ReadBool();
            PageSize = packet.ReadInt();
            PageIndex = packet.ReadInt();
            SearchType = (GroupMemberSearchType)packet.ReadInt();
            Filter = packet.ReadString();
        }
    }
}
