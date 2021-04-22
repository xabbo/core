using System;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public class GroupInfo : IGroupInfo
    {
        public static GroupInfo Parse(IReadOnlyPacket packet) => new GroupInfo(packet);

        public long Id { get; set; }
        public string Name { get; set; }
        public string BadgeCode { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public bool IsFavorite { get; set; }
        public long OwnerId { get; set; }
        public bool HasForum { get; set; }

        public GroupInfo() { }

        protected GroupInfo(IReadOnlyPacket packet)
        {
            Id = packet.ReadLegacyLong();
            Name = packet.ReadString();
            BadgeCode = packet.ReadString();
            PrimaryColor = packet.ReadString();
            SecondaryColor = packet.ReadString();
            IsFavorite = packet.ReadBool();
            OwnerId = packet.ReadLegacyLong();
            HasForum = packet.ReadBool();
        }

        public void Compose(IPacket packet)
        {
            packet.WriteLegacyLong(Id);
            packet.WriteString(Name);
            packet.WriteString(BadgeCode);
            packet.WriteString(PrimaryColor);
            packet.WriteString(SecondaryColor);
            packet.WriteBool(IsFavorite);
            packet.WriteLegacyLong(OwnerId);
            packet.WriteBool(HasForum);
        }
    }
}
