using System;
using System.Collections.Generic;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public class PetInfo : IComposable
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public int MaxLevel { get; set; }
        public int Experience { get; set; }
        public int MaxExperience { get; set; }
        public int Energy { get; set; }
        public int MaxEnergy { get; set; }
        public int Happiness { get; set; }
        public int MaxHappiness { get; set; }
        public int Scratches { get; set; }
        public int OwnerId { get; set; }
        public int Age { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public int Breed { get; set; }
        public bool HasFreeSaddle { get; set; }
        public bool IsRiding { get; set; }
        public List<int> SkillThresholds { get; set; } = new(); // Something to do with skills (horse)
        public int AccessRights { get; set; }
        public bool CanBreed { get; set; }
        public bool CanHarvest { get; set; }
        public bool CanRevive { get; set; }
        public int RarityLevel { get; set; }
        public int MaxWellbeingSeconds { get; set; }
        public int RemainingWellbeingSeconds { get; set; }
        public int RemainingGrowingSeconds { get; set; }
        public bool HasBreedingPermission { get; set; }

        public PetInfo() { }

        protected PetInfo(IReadOnlyPacket packet)
            : this()
        {
            Id = packet.ReadInt();
            Name = packet.ReadString();
            Level = packet.ReadInt();
            MaxLevel = packet.ReadInt();
            Experience = packet.ReadInt();
            MaxExperience = packet.ReadInt();
            Energy = packet.ReadInt();
            MaxEnergy = packet.ReadInt();
            Happiness = packet.ReadInt();
            MaxHappiness = packet.ReadInt();
            Scratches = packet.ReadInt();
            OwnerId = packet.ReadInt();
            Age = packet.ReadInt();
            OwnerName = packet.ReadString();
            Breed = packet.ReadInt();
            HasFreeSaddle = packet.ReadBool();
            IsRiding = packet.ReadBool();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                SkillThresholds.Add(packet.ReadInt());

            AccessRights = packet.ReadInt();
            CanBreed = packet.ReadBool();
            CanHarvest = packet.ReadBool();
            CanRevive = packet.ReadBool();
            RarityLevel = packet.ReadInt();
            MaxWellbeingSeconds = packet.ReadInt();
            RemainingWellbeingSeconds = packet.ReadInt();
            RemainingGrowingSeconds = packet.ReadInt();
            HasBreedingPermission = packet.ReadBool();
        }

        public void Compose(IPacket packet)
        {
            packet.WriteInt(Id);
            packet.WriteString(Name);
            packet.WriteInt(Level);
            packet.WriteInt(MaxLevel);
            packet.WriteInt(Experience);
            packet.WriteInt(MaxExperience);
            packet.WriteInt(Energy);
            packet.WriteInt(MaxEnergy);
            packet.WriteInt(Happiness);
            packet.WriteInt(MaxHappiness);
            packet.WriteInt(Scratches);
            packet.WriteInt(OwnerId);
            packet.WriteInt(Age);
            packet.WriteString(OwnerName);
            packet.WriteInt(Breed);
            packet.WriteBool(HasFreeSaddle);
            packet.WriteBool(IsRiding);

            packet.WriteInt(SkillThresholds.Count);
            foreach (int value in SkillThresholds)
                packet.WriteInt(value);

            packet.WriteInt(AccessRights);
            packet.WriteBool(CanBreed);
            packet.WriteBool(CanHarvest);
            packet.WriteBool(CanRevive);
            packet.WriteInt(RarityLevel);
            packet.WriteInt(MaxWellbeingSeconds);
            packet.WriteInt(RemainingWellbeingSeconds);
            packet.WriteInt(RemainingGrowingSeconds);
            packet.WriteBool(HasBreedingPermission);
        }

        public static PetInfo Parse(IReadOnlyPacket packet) => new(packet);
    }
}
