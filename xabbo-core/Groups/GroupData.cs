using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class GroupData
    {
        public static GroupData Parse(Packet packet) => new GroupData(packet);

        public int Id { get; set; }
        public bool CanLeave { get; set; }
        public GroupType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BadgeCode { get; set; }
        public int HomeRoomId { get; set; }
        public string HomeRoomName { get; set; }
        public GroupMemberStatus MemberStatus { get; set; }
        public int MemberCount { get; set; }
        public bool IsFavourite { get; set; }
        public string Created { get; set; }
        public bool IsOwner { get; set; }
        public bool IsAdmin { get; set; }
        public string OwnerName { get; set; }
        public bool UnknownBoolA { get; set; }
        public bool CanDecorateHomeRoom { get; set; }
        public int PendingRequests { get; set; }
        public bool CanViewForum { get; set; }

        protected GroupData(Packet packet)
        {
            Id = packet.ReadInteger();
            CanLeave = packet.ReadBoolean();
            Type = (GroupType)packet.ReadInteger();
            Name = packet.ReadString();
            Description = packet.ReadString();
            BadgeCode = packet.ReadString();
            HomeRoomId = packet.ReadInteger();
            HomeRoomName = packet.ReadString();
            MemberStatus = (GroupMemberStatus)packet.ReadInteger();
            MemberCount = packet.ReadInteger();
            IsFavourite = packet.ReadBoolean();
            Created = packet.ReadString();
            IsOwner = packet.ReadBoolean();
            IsAdmin = packet.ReadBoolean();
            OwnerName = packet.ReadString();
            UnknownBoolA = packet.ReadBoolean();
            CanDecorateHomeRoom = packet.ReadBoolean();
            PendingRequests = packet.ReadInteger();
            CanViewForum = packet.ReadBoolean();
        }
    }

    public enum GroupType
    {
        Open = 0,
        Exclusive = 1,
        Closed = 2
    }

    public enum GroupMemberStatus
    {
        NotAMember = 0,
        Member = 1,
        Requested = 2
    }
}
