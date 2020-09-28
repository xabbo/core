using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class FriendInfo : IFriendInfo, IWritable
    {
        public static FriendInfo Parse(Packet packet) => new FriendInfo(packet);

        public int Id { get; set; }
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

        public FriendInfo() { }

        protected FriendInfo(Packet packet)
        {
            Id = packet.ReadInt();
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

        void IWritable.Write(Packet packet)
        {
            packet.WriteValues(
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
        }
    }
}
