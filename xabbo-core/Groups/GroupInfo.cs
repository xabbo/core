using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class GroupInfo
    {
        public static GroupInfo Parse(Packet packet) => new GroupInfo(packet);

        public int Id { get; set; }
        public string Name { get; set; }
        public string Badge { get; set; }
        public string ColorA { get; set; }
        public string ColorB { get; set; }
        public bool IsFavorite { get; set; }
        public int OwnerId { get; set; }
        public bool UnknownBool1 { get; set; }

        protected GroupInfo(Packet packet)
        {
            Id = packet.ReadInteger();
            Name = packet.ReadString();
            Badge = packet.ReadString();
            ColorA = packet.ReadString();
            ColorB = packet.ReadString();
            IsFavorite = packet.ReadBoolean();
            OwnerId = packet.ReadInteger();
            UnknownBool1 = packet.ReadBoolean();
        }
    }
}
