using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class UserSearchResult
    {
        public static UserSearchResult Parse(Packet packet) => new UserSearchResult(packet);

        public int Id { get; set; }
        public string Name { get; set; }
        public string Motto { get; set; }
        public bool Online { get; set; }
        public bool UnknownBoolA { get; set; }
        public string UnknownStringA { get; set; }
        public int UnknownIntA { get; set; }
        public string Figure { get; set; }
        public string RealName { get; set; }

        public UserSearchResult() { }

        protected UserSearchResult(Packet packet)
        {
            Id = packet.ReadInteger();
            Name = packet.ReadString();
            Motto = packet.ReadString();
            Online = packet.ReadBoolean();
            UnknownBoolA = packet.ReadBoolean();
            UnknownStringA = packet.ReadString();
            UnknownIntA = packet.ReadInteger();
            Figure  = packet.ReadString();
            RealName = packet.ReadString();
        }
    }
}
