using System;
using Xabbo.Core;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Friend : IFriend
    {
        public static Friend Parse(IReadOnlyPacket packet) => new Friend(packet);

        public long Id { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public bool IsOnline { get; set; }
        public bool CanFollow { get; set; }
        public string FigureString { get; set; }
        public int Category { get; set; }
        public string Motto { get; set; }
        public string RealName { get; set; }
        public string String5 { get; set; }
        public bool IsAcceptingOfflineMessages { get; set; }
        public bool Bool4 { get; set; }
        public bool IsPocketHabboUser { get; set; }
        public Relation Relation { get; set; }

        public Friend() { }

        protected Friend(IReadOnlyPacket packet)
        {
            Id = packet.ReadLong();
            Name = packet.ReadString();
            Gender = H.ToGender(packet.ReadInt());
            IsOnline = packet.ReadBool();
            CanFollow = packet.ReadBool();
            FigureString = packet.ReadString();
            Category = packet.ReadInt();
            Motto = packet.ReadString();
            RealName = packet.ReadString();
            String5 = packet.ReadString();
            IsAcceptingOfflineMessages = packet.ReadBool();
            Bool4 = packet.ReadBool();
            IsPocketHabboUser = packet.ReadBool();
            Relation = (Relation)packet.ReadShort();
        }

        public void Write(IPacket packet) => packet.WriteValues(
            Id,
            Name,
            Gender.GetValue(),
            IsOnline,
            CanFollow,
            FigureString,
            Category,
            Motto,
            RealName,
            String5,
            IsAcceptingOfflineMessages,
            Bool4,
            IsPocketHabboUser,
            (short)Relation
        );

        public override string ToString() => $"{Name} (id:{Id})";
    }
}
