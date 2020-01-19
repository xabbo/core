using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class UserData
    {/*
		    int userId
		    string userName
		    string figureString
		    string gender
		    string motto?
		    string ?
		    bool ?
		    int ?
		    int respectsLeft
		    int scratchesLeft
		    bool ?
		    string lastLogin
		    bool nameChangeable
		    bool ?
            */
         //UserData 65361601 ",,b7" "hr-679-42.hd-180-1.ch-3110-64-1408.lg-275-64.ha-1003-64.ea-1406.fa-1212" "M" "国際刑事警察機構"
         // "" false 28 0 3 true "11-11-2019 17:25:02" false false

        public int Id { get; set; }
        public string Name { get; set; }
        public string Figure { get; set; }
        public Gender Gender { get; set; }
        public string Motto { get; set; }
        public string StringA { get; set; }
        public bool BoolA { get; set; }
        public int IntA { get; set; }
        public int RespectsLeft { get; set; }
        public int ScratchesLeft { get; set; }
        public bool BoolB { get; set; }
        public string LastLogin { get; set; }
        public bool NameChangeable { get; set; }
        public bool BoolC { get; set; }

        public UserData() { }

        private UserData(Packet packet)
        {
            Id = packet.ReadInteger();
            Name = packet.ReadString();
            Figure = packet.ReadString();
            Gender = H.ToGender(packet.ReadString());
            Motto = packet.ReadString();
            StringA = packet.ReadString();
            BoolA = packet.ReadBoolean();
            IntA = packet.ReadInteger();
            RespectsLeft = packet.ReadInteger();
            ScratchesLeft = packet.ReadInteger();
            BoolB = packet.ReadBoolean();
            LastLogin = packet.ReadString();
            NameChangeable = packet.ReadBoolean();
            BoolC = packet.ReadBoolean();
        }

        public static UserData Parse(Packet packet) => new UserData(packet);
    }
}
