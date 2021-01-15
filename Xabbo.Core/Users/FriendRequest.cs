using System;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class FriendRequest
    {
        public static FriendRequest Parse(Packet packet) => new FriendRequest(packet);

        public int Id { get; set; }
        public string Name { get; set; }
        public string Figure { get; set; }

        public FriendRequest() { }

        protected FriendRequest(Packet packet)
        {
            Id = packet.ReadInt();
            Name = packet.ReadString();
            Figure = packet.ReadString();
        }
    }
}
