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
        public bool Bool1 { get; set; }
        public bool Bool2 { get; set; }
        public bool Bool3 { get; set; }
        public bool Bool4 { get; set; }
        public bool Bool5 { get; set; }
        public bool Bool6 { get; set; }
        public int Level { get; set; }
        public string Stance { get; set; }

        public Pet(long id, int index)
            : base(EntityType.Pet, id, index)
        {
            OwnerId = -1;
            OwnerName = "(unknown)";
            Stance = "";
        }

        internal Pet(long id, int index, IReadOnlyPacket packet)
            : this(id, index)
        {
            Breed = packet.ReadInt();
            OwnerId = packet.ReadLegacyLong();
            OwnerName = packet.ReadString();
            RarityLevel = packet.ReadInt();
            Bool1 = packet.ReadBool();
            Bool2 = packet.ReadBool();
            Bool3 = packet.ReadBool();
            Bool4 = packet.ReadBool();
            Bool5 = packet.ReadBool();
            Bool6 = packet.ReadBool();
            Level = packet.ReadInt();
            Stance = packet.ReadString();
        }

        public override void Compose(IPacket packet)
        {
            base.Compose(packet);

            packet
                .WriteInt(Breed)
                .WriteLegacyLong(OwnerId)
                .WriteInt(RarityLevel)
                .WriteBool(Bool1)
                .WriteBool(Bool2)
                .WriteBool(Bool3)
                .WriteBool(Bool4)
                .WriteBool(Bool5)
                .WriteBool(Bool6)
                .WriteInt(Level)
                .WriteString(Stance);
        }
    }
}
