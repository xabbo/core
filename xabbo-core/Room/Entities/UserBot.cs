using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class UserBot : Entity
    {
        public Gender Gender { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public List<short> Unknown1 { get; set; }

        public UserBot(int id, int index)
            : base(EntityType.UserBot, id, index)
        {
            Unknown1 = new List<short>();
        }

        internal UserBot(int id, int index, Packet packet)
            : this(id, index)
        {
            Gender = H.ToGender(packet.ReadString());
            OwnerId = packet.ReadInteger();
            OwnerName = packet.ReadString();

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                Unknown1.Add(packet.ReadShort());
        }
    }
}
