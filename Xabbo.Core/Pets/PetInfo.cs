using System;
using System.Collections.Generic;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class PetInfo : IPacketData
    {
        public static PetInfo Parse(IReadOnlyPacket packet) => new PetInfo(packet);

        public int Id { get; set; }
        public string Name { get; set; }
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
        public string OwnerName { get; set; }
        public int Breed { get; set; }
        public bool UnknownBoolA { get; set; }
        public bool UnknownBoolB { get; set; }
        public List<int> UnknownIntsA { get; set; } // Something to do with skills (horse)
        public int UnknownIntC { get; set; }
        public bool UnknownBoolC { get; set; }
        public bool UnknownBoolD { get; set; }
        public bool UnknownBoolE { get; set; }
        public int RarityLevel { get; set; }
        public int MaxRemainingWellbeing { get; set; }
        public int RemainingWellbeing { get; set; }
        public int TimeUntilGrownUp { get; set; }
        public bool UnknownBoolF { get; set; }

        public PetInfo()
        {
            UnknownIntsA = new List<int>();
        }

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
            UnknownBoolA = packet.ReadBool();
            UnknownBoolB = packet.ReadBool();

            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                UnknownIntsA.Add(packet.ReadInt());

            UnknownIntC = packet.ReadInt();
            UnknownBoolC = packet.ReadBool();
            UnknownBoolD = packet.ReadBool();
            UnknownBoolE = packet.ReadBool();
            RarityLevel = packet.ReadInt();
            MaxRemainingWellbeing = packet.ReadInt();
            RemainingWellbeing = packet.ReadInt();
            TimeUntilGrownUp = packet.ReadInt();
            UnknownBoolF = packet.ReadBool();
        }

        public void Write(IPacket packet)
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
            packet.WriteBool(UnknownBoolA);
            packet.WriteBool(UnknownBoolB);

            packet.WriteInt(UnknownIntsA.Count);
            foreach (int value in UnknownIntsA)
                packet.WriteInt(value);

            packet.WriteInt(UnknownIntC);
            packet.WriteBool(UnknownBoolC);
            packet.WriteBool(UnknownBoolD);
            packet.WriteBool(UnknownBoolE);
            packet.WriteInt(RarityLevel);
            packet.WriteInt(MaxRemainingWellbeing);
            packet.WriteInt(RemainingWellbeing);
            packet.WriteInt(TimeUntilGrownUp);
            packet.WriteBool(UnknownBoolF);
        }
    }
}
