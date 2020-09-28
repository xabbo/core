using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Pet : Entity, IPet
    {
        public int Breed { get; set; }
        public int OwnerId { get; set; }
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

        public Pet(int id, int index)
            : base(EntityType.Pet, id, index)
        {
            OwnerId = -1;
            OwnerName = "(unknown)";
            Stance = "";
        }

        internal Pet(int id, int index, Packet packet)
            : this(id, index)
        {
            Breed = packet.ReadInt();
            OwnerId = packet.ReadInt();
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

        public override void Write(Packet packet)
        {
            base.Write(packet);

            packet.WriteValues(
                Breed,
                OwnerId,
                OwnerName,
                RarityLevel,
                Bool1,
                Bool2,
                Bool3,
                Bool4,
                Bool5,
                Bool6,
                Level,
                Stance
            );
        }
    }
}
