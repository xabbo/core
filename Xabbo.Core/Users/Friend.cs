using System;

using Xabbo.Common;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public class Friend : IFriend
    {
        public static Friend Parse(IReadOnlyPacket packet) => new(packet);

        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public bool IsOnline { get; set; }
        public bool CanFollow { get; set; }
        public string FigureString { get; set; } = string.Empty;
        public long CategoryId { get; set; }
        public string Motto { get; set; } = string.Empty;
        public string RealName { get; set; } = string.Empty;
        public string FacebookId { get; set; } = string.Empty;
        public bool IsAcceptingOfflineMessages { get; set; }
        public bool IsVipMember { get; set; }
        public bool IsPocketHabboUser { get; set; }
        public Relation Relation { get; set; }

        public Friend() { }

        protected Friend(IReadOnlyPacket packet)
        {
            Id = packet.ReadLegacyLong();
            Name = packet.ReadString();
            Gender = H.ToGender(packet.ReadInt());
            IsOnline = packet.ReadBool();
            CanFollow = packet.ReadBool();
            FigureString = packet.ReadString();
            CategoryId = packet.ReadLegacyLong();
            Motto = packet.ReadString();

            if (packet.Protocol == ClientType.Flash)
            {
                RealName = packet.ReadString();
                FacebookId = packet.ReadString();
            }

            IsAcceptingOfflineMessages = packet.ReadBool();
            IsVipMember = packet.ReadBool();
            IsPocketHabboUser = packet.ReadBool();

            if (packet.Protocol == ClientType.Unity)
            {
                RealName = packet.ReadString();
                FacebookId = packet.ReadString();
            }

            Relation = (Relation)packet.ReadShort();
        }

        public void Compose(IPacket packet) => packet
            .WriteLegacyLong((LegacyLong)Id)
            .WriteString(Name)
            .WriteInt(Gender.GetValue())
            .WriteBool(IsOnline)
            .WriteBool(CanFollow)
            .WriteString(FigureString)
            .WriteLegacyLong(CategoryId)
            .WriteString(Motto)
            .WriteString(RealName)
            .WriteString(FacebookId)
            .WriteBool(IsAcceptingOfflineMessages)
            .WriteBool(IsVipMember)
            .WriteBool(IsPocketHabboUser)
            .WriteShort((short)Relation);

        public override string ToString() => Name;
    }
}
