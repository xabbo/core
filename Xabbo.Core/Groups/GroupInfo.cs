using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class GroupInfo : IGroupInfo
    {
        public static GroupInfo Parse(IReadOnlyPacket packet) => new GroupInfo(packet);

        public int Id { get; set; }
        public string Name { get; set; }
        public string BadgeCode { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public bool IsFavorite { get; set; }
        public int OwnerId { get; set; }
        public bool HasForum { get; set; }

        public GroupInfo() { }

        protected GroupInfo(IReadOnlyPacket packet)
        {
            Id = packet.ReadInt();
            Name = packet.ReadString();
            BadgeCode = packet.ReadString();
            PrimaryColor = packet.ReadString();
            SecondaryColor = packet.ReadString();
            IsFavorite = packet.ReadBool();
            OwnerId = packet.ReadInt();
            HasForum = packet.ReadBool();
        }

        public void Write(IPacket packet)
        {
            packet.WriteInt(Id);
            packet.WriteString(Name);
            packet.WriteString(BadgeCode);
            packet.WriteString(PrimaryColor);
            packet.WriteString(SecondaryColor);
            packet.WriteBool(IsFavorite);
            packet.WriteInt(OwnerId);
            packet.WriteBool(HasForum);
        }
    }
}
