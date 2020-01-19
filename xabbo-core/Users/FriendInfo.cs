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
        public int Gender { get; set; }
        public bool IsOnline { get; set; }
        public bool CanFollow { get; set; } // @Sulakore
        public string FigureString { get; set; }
        public int UnknownIntA { get; set; } // CategoryId @Sulakore
        public string Motto { get; set; }
        public string RealName { get; set; }
        public string UnknownStringA { get; set; }
        public bool UnknownBoolB { get; set; } // IsPersisted @Sulakore
        public bool UnknownBoolC { get; set; }
        public bool IsPocketHabboUser { get; set; }
        public Relation Relation { get; set; } // @Sulakore

        public FriendInfo() { }

        protected FriendInfo(Packet packet)
        {
            Id = packet.ReadInteger();
            Name = packet.ReadString();
            Gender = packet.ReadInteger(); // H.ToGender(packet.ReadInteger());
            IsOnline = packet.ReadBoolean();
            CanFollow = packet.ReadBoolean();
            FigureString = packet.ReadString();
            UnknownIntA = packet.ReadInteger();
            Motto = packet.ReadString();
            RealName = packet.ReadString();
            UnknownStringA = packet.ReadString();
            UnknownBoolB = packet.ReadBoolean();
            UnknownBoolC = packet.ReadBoolean();
            IsPocketHabboUser = packet.ReadBoolean();
            Relation = (Relation)packet.ReadShort();
        }

        void IWritable.Write(Packet packet)
        {
            packet.WriteValues(
                Id,
                Name,
                Gender,
                IsOnline,
                CanFollow,
                FigureString,
                UnknownIntA,
                Motto,
                RealName,
                UnknownStringA,
                UnknownBoolB,
                UnknownBoolC,
                IsPocketHabboUser,
                (int)Relation
            );
        }
    }
}
