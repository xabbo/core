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
        public bool UnknownBoolA { get; set; }
        public bool UnknownBoolB { get; set; }
        public bool UnknownBoolC { get; set; }
        public bool UnknownBoolD { get; set; }
        public bool UnknownBoolE { get; set; }
        public bool UnknownBoolF { get; set; }
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
            UnknownBoolA = packet.ReadBoolean();
            UnknownBoolB = packet.ReadBoolean();
            UnknownBoolC = packet.ReadBoolean();
            UnknownBoolD = packet.ReadBoolean();
            UnknownBoolE = packet.ReadBoolean();
            UnknownBoolF = packet.ReadBoolean();
            Level = packet.ReadInteger();
            Stance = packet.ReadString();
        }

        public override void Write(Packet packet)
        {
            base.Write(packet);

            packet.WriteValues(
                Breed,
                OwnerId,
                OwnerName,
                RarityLevel,
                UnknownBoolA,
                UnknownBoolB,
                UnknownBoolC,
                UnknownBoolD,
                UnknownBoolE,
                UnknownBoolF,
                Level,
                Stance
            );
        }
    }
}
