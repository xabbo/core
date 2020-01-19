using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Pet : Entity
    {
        public int Breed { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public int RarityLevel { get; set; }
        public bool UnknownBool1 { get; set; }
        public bool UnknownBool2 { get; set; }
        public bool UnknownBool3 { get; set; }
        public bool UnknownBool4 { get; set; }
        public bool UnknownBool5 { get; set; }
        public bool UnknownBool6 { get; set; }
        public int Level { get; set; }
        public string Stance { get; set; }

        public Pet(int id, int index)
            : base(EntityType.Pet, id, index)
        { }

        internal Pet(int id, int index, Packet packet)
            : this(id, index)
        {
            Breed = packet.ReadInteger();
            OwnerId = packet.ReadInteger();
            OwnerName = packet.ReadString();
            RarityLevel = packet.ReadInteger();
            UnknownBool1 = packet.ReadBoolean();
            UnknownBool2 = packet.ReadBoolean();
            UnknownBool3 = packet.ReadBoolean();
            UnknownBool4 = packet.ReadBoolean();
            UnknownBool5 = packet.ReadBoolean();
            UnknownBool6 = packet.ReadBoolean();
            Level = packet.ReadInteger();
            Stance = packet.ReadString();
        }
    }
}
