using System;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public class FriendRequest
    {
        public static FriendRequest Parse(Packet packet) => new(packet);

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Figure { get; set; } = string.Empty;

        public FriendRequest() { }

        protected FriendRequest(Packet packet)
        {
            Id = packet.ReadInt();
            Name = packet.ReadString();
            Figure = packet.ReadString();
        }
    }
}
