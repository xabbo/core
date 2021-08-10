using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class Pet : Entity, IPet
    {
        public int Breed { get; set; }
        public long OwnerId { get; set; }
        public string OwnerName { get; set; }
        public int RarityLevel { get; set; }
        public bool HasSaddle { get; set; }
        public bool IsRiding { get; set; }
        public bool CanBreed { get; set; }
        public bool CanHarvest { get; set; }
        public bool CanRevive { get; set; }
        public bool HasBreedingPermission { get; set; }
        public int Level { get; set; }
        public string Posture { get; set; }

        public Pet(long id, int index)
            : base(EntityType.Pet, id, index)
        {
            OwnerId = -1;
            OwnerName = "(unknown)";
            Posture = "";
        }

        internal Pet(long id, int index, IReadOnlyPacket packet)
            : this(id, index)
        {
            Breed = packet.ReadInt();
            OwnerId = packet.ReadLegacyLong();
            OwnerName = packet.ReadString();
            RarityLevel = packet.ReadInt();
            HasSaddle = packet.ReadBool();
            IsRiding = packet.ReadBool();
            CanBreed = packet.ReadBool();
            CanHarvest = packet.ReadBool();
            CanRevive = packet.ReadBool();
            HasBreedingPermission = packet.ReadBool();
            Level = packet.ReadInt();
            Posture = packet.ReadString();
        }

        public override void Compose(IPacket packet)
        {
            base.Compose(packet);

            packet
                .WriteInt(Breed)
                .WriteLegacyLong(OwnerId)
                .WriteInt(RarityLevel)
                .WriteBool(HasSaddle)
                .WriteBool(IsRiding)
                .WriteBool(CanBreed)
                .WriteBool(CanHarvest)
                .WriteBool(CanRevive)
                .WriteBool(HasBreedingPermission)
                .WriteInt(Level)
                .WriteString(Posture);
        }
    }
}
