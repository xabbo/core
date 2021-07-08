using System;
using Xabbo.Core;
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
        public long Category { get; set; }
        public string Motto { get; set; } = string.Empty;
        public string RealName { get; set; } = string.Empty;
        public string String5 { get; set; } = string.Empty;
        public bool IsAcceptingOfflineMessages { get; set; }
        public bool Bool4 { get; set; }
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
            Category = packet.ReadLegacyLong();
            Motto = packet.ReadString();

            if (packet.Protocol == ClientType.Flash)
            {
                RealName = packet.ReadString();
                String5 = packet.ReadString();
            }

            IsAcceptingOfflineMessages = packet.ReadBool();
            Bool4 = packet.ReadBool();
            IsPocketHabboUser = packet.ReadBool();

            if (packet.Protocol == ClientType.Unity)
            {
                RealName = packet.ReadString();
                String5 = packet.ReadString();
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
            .WriteLegacyLong(Category)
            .WriteString(Motto)
            .WriteString(RealName)
            .WriteString(String5)
            .WriteBool(IsAcceptingOfflineMessages)
            .WriteBool(Bool4)
            .WriteBool(IsPocketHabboUser)
            .WriteShort((short)Relation);

        public override string ToString() => Name;
    }
}
