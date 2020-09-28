using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class GroupInfo : IGroupInfo
    {
        public static GroupInfo Parse(IReadOnlyPacket packet) => new GroupInfo(packet);

        public int Id { get; set; }
        public string Name { get; set; }
        public string Badge { get; set; }
        public string ColorA { get; set; }
        public string ColorB { get; set; }
        public bool IsFavorite { get; set; }
        public int OwnerId { get; set; }
        public bool Bool2 { get; set; }

        protected GroupInfo(IReadOnlyPacket packet)
        {
            Id = packet.ReadInt();
            Name = packet.ReadString();
            Badge = packet.ReadString();
            ColorA = packet.ReadString();
            ColorB = packet.ReadString();
            IsFavorite = packet.ReadBool();
            OwnerId = packet.ReadInt();
            Bool2 = packet.ReadBool();
        }
    }
}
