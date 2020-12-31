using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class UserSearchResult
    {
        public static UserSearchResult Parse(IReadOnlyPacket packet) => new UserSearchResult(packet);

        public long Id { get; set; }
        public string Name { get; set; }
        public string Motto { get; set; }
        public bool Online { get; set; }
        public bool UnknownBoolA { get; set; }
        public string UnknownStringA { get; set; }
        public int UnknownIntA { get; set; }
        public string Figure { get; set; }
        public string RealName { get; set; }

        public UserSearchResult() { }

        protected UserSearchResult(IReadOnlyPacket packet)
        {
            Id = packet.ReadLong();
            Name = packet.ReadString();
            Motto = packet.ReadString();
            Online = packet.ReadBool();
            UnknownBoolA = packet.ReadBool();
            UnknownStringA = packet.ReadString();
            UnknownIntA = packet.ReadInt();
            Figure  = packet.ReadString();
            RealName = packet.ReadString();
        }
    }
}
