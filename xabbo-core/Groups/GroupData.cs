using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class GroupData : IGroupData
    {
        public static GroupData Parse(IReadOnlyPacket packet) => new GroupData(packet);

        public int Id { get; set; }
        public bool CanLeave { get; set; }
        public GroupType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Badge { get; set; }
        public int HomeRoomId { get; set; }
        public string HomeRoomName { get; set; }
        public GroupMemberStatus MemberStatus { get; set; }
        public int MemberCount { get; set; }
        public bool IsFavourite { get; set; }
        public string Created { get; set; }
        public bool IsOwner { get; set; }
        public bool IsAdmin { get; set; }
        public string OwnerName { get; set; }
        public bool Bool5 { get; set; }
        public bool CanDecorateHomeRoom { get; set; }
        public int PendingRequests { get; set; }
        public bool CanViewForum { get; set; }

        protected GroupData(IReadOnlyPacket packet)
        {
            Id = packet.ReadInt();
            CanLeave = packet.ReadBool();
            Type = (GroupType)packet.ReadInt();
            Name = packet.ReadString();
            Description = packet.ReadString();
            Badge = packet.ReadString();
            HomeRoomId = packet.ReadInt();
            HomeRoomName = packet.ReadString();
            MemberStatus = (GroupMemberStatus)packet.ReadInt();
            MemberCount = packet.ReadInt();
            IsFavourite = packet.ReadBool();
            Created = packet.ReadString();
            IsOwner = packet.ReadBool();
            IsAdmin = packet.ReadBool();
            OwnerName = packet.ReadString();
            Bool5 = packet.ReadBool();
            CanDecorateHomeRoom = packet.ReadBool();
            PendingRequests = packet.ReadInt();
            CanViewForum = packet.ReadBool();
        }
    }
}
