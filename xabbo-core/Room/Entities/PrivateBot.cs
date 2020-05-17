using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class PrivateBot : Entity
    {
        public Gender Gender { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public List<short> UnknownShortsA { get; set; }

        public PrivateBot(int id, int index)
            : base(EntityType.PrivateBot, id, index)
        {
            UnknownShortsA = new List<short>();
        }

        internal PrivateBot(int id, int index, Packet packet)
            : this(id, index)
        {
            Gender = H.ToGender(packet.ReadString());
            OwnerId = packet.ReadInteger();
            OwnerName = packet.ReadString();

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                UnknownShortsA.Add(packet.ReadShort());
        }

        public override void Write(Packet packet)
        {
            base.Write(packet);

            packet.WriteValues(
                Gender.ToShortString(),
                OwnerId,
                OwnerName,
                UnknownShortsA
            );
        }
    }
}
