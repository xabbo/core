using System;
using Xabbo.Core.Messages;

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

        protected Badge(IReadOnlyPacket packet)
        {
            Id = packet.ReadInt();
            Code = packet.ReadString();
        }

        public static Badge Parse(IReadOnlyPacket packet) => new Badge(packet);
    }
}
