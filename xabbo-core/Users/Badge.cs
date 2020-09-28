using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Badge
    {
        public int Id { get; set; }
        public string Code { get; set; }

        public Badge() { }

        public Badge(int id, string code)
        {
            Id = id;
            Code = code;
        }

        protected Badge(Packet packet)
        {
            Id = packet.ReadInt();
            Code = packet.ReadString();
        }

        public static Badge Parse(Packet packet) => new Badge(packet);
    }
}
