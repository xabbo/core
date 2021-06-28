using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class GroupInfo : IGroupInfo
    {
        public static GroupInfo Parse(IReadOnlyPacket packet) => new GroupInfo(packet);

        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BadgeCode { get; set; } = string.Empty;
        public string PrimaryColor { get; set; } = string.Empty;
        public string SecondaryColor { get; set; } = string.Empty;
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
            packet
                .WriteLegacyLong(Id)
                .WriteString(Name)
                .WriteString(BadgeCode)
                .WriteString(PrimaryColor)
                .WriteString(SecondaryColor)
                .WriteBool(IsFavorite)
                .WriteLegacyLong(OwnerId)
                .WriteBool(HasForum);
        }
    }
}
