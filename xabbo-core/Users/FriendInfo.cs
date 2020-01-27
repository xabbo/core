using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class FriendInfo : IWritable
    {
        public static FriendInfo Parse(Packet packet) => new FriendInfo(packet);

        /*
            int id
            string name
            int gender
            bool online
            bool ? 
            string figure
            int ?
            string motto
            string realName
            string ?
            bool 
            bool 
            bool packetHabboUser
            short 
        */

        public int Id { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public bool IsOnline { get; set; }
        public bool CanFollow { get; set; }
        public string FigureString { get; set; }
        public int Category { get; set; }
        public string Motto { get; set; }
        public string RealName { get; set; }
        public string UnknownStringA { get; set; }
        public bool IsAcceptingOfflineMessages { get; set; }
        public bool UnknownBoolC { get; set; }
        public bool IsPocketHabboUser { get; set; }
        public Relation Relation { get; set; }

        public FriendInfo() { }

        protected FriendInfo(Packet packet)
        {
            Id = packet.ReadInteger();
            Name = packet.ReadString();
            Gender = H.ToGender(packet.ReadInteger());
            IsOnline = packet.ReadBoolean();
            CanFollow = packet.ReadBoolean();
            FigureString = packet.ReadString();
            Category = packet.ReadInteger();
            Motto = packet.ReadString();
            RealName = packet.ReadString();
            UnknownStringA = packet.ReadString();
            IsAcceptingOfflineMessages = packet.ReadBoolean();
            UnknownBoolC = packet.ReadBoolean();
            IsPocketHabboUser = packet.ReadBoolean();
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
                UnknownStringA,
                IsAcceptingOfflineMessages,
                UnknownBoolC,
                IsPocketHabboUser,
                (short)Relation
            );
        }
    }
}
