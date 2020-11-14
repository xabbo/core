using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    /// <summary>
    /// The user's own data that is sent upon requesting user data.
    /// </summary>
    public class UserData : IUserData
    {
        public static UserData Parse(IReadOnlyPacket packet) => new UserData(packet);

        public int Id { get; set; }
        public string Name { get; set; }
        public string Figure { get; set; }
        public Gender Gender { get; set; }
        public string Motto { get; set; }
        public string String4 { get; set; } // RealName @Sulakore
        public bool Bool1 { get; set; } // DirectMail @Sulakore
        public int TotalRespects { get; set; }
        public int RespectsLeft { get; set; }
        public int ScratchesLeft { get; set; }
        public bool Bool2 { get; set; } // StreamPublishingAllowed @Sulakore
        public string LastLogin { get; set; }
        public bool IsNameChangeable { get; set; }
        public bool IsSafetyLocked { get; set; }

        public UserData() { }

        protected UserData(IReadOnlyPacket packet)
        {
            Id = packet.ReadInt();
            Name = packet.ReadString();
            Figure = packet.ReadString();
            Gender = H.ToGender(packet.ReadString());
            Motto = packet.ReadString();
            String4 = packet.ReadString();
            Bool1 = packet.ReadBool();
            TotalRespects = packet.ReadInt();
            RespectsLeft = packet.ReadInt();
            ScratchesLeft = packet.ReadInt();
            Bool2 = packet.ReadBool();
            LastLogin = packet.ReadString();
            IsNameChangeable = packet.ReadBool();
            IsSafetyLocked = packet.ReadBool();
        }

        public void Write(Packet packet) => packet.WriteValues(
            Id,
            Name,
            Figure,
            Gender.ToShortString(),
            Motto,
            String4,
            Bool1,
            TotalRespects,
            RespectsLeft,
            ScratchesLeft,
            Bool2,
            LastLogin,
            IsNameChangeable,
            IsSafetyLocked
        );
    }
}
