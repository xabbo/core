using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class PetInfo : IWritable
    {
        public static PetInfo Parse(Packet packet) => new PetInfo(packet);

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

        protected PetInfo(Packet packet)
            : this()
        {
            Id = packet.ReadInteger();
            Name = packet.ReadString();
            Level = packet.ReadInteger();
            MaxLevel = packet.ReadInteger();
            Experience = packet.ReadInteger();
            MaxExperience = packet.ReadInteger();
            Energy = packet.ReadInteger();
            MaxEnergy = packet.ReadInteger();
            Happiness = packet.ReadInteger();
            MaxHappiness = packet.ReadInteger();
            Scratches = packet.ReadInteger();
            OwnerId = packet.ReadInteger();
            Age = packet.ReadInteger();
            OwnerName = packet.ReadString();
            Breed = packet.ReadInteger();
            UnknownBoolA = packet.ReadBoolean();
            UnknownBoolB = packet.ReadBoolean();

            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                UnknownIntsA.Add(packet.ReadInteger());

            UnknownIntC = packet.ReadInteger();
            UnknownBoolC = packet.ReadBoolean();
            UnknownBoolD = packet.ReadBoolean();
            UnknownBoolE = packet.ReadBoolean();
            RarityLevel = packet.ReadInteger();
            MaxRemainingWellbeing = packet.ReadInteger();
            RemainingWellbeing = packet.ReadInteger();
            TimeUntilGrownUp = packet.ReadInteger();
            UnknownBoolF = packet.ReadBoolean();
        }

        public void Write(Packet packet)
        {
            packet.WriteInteger(Id);
            packet.WriteString(Name);
            packet.WriteInteger(Level);
            packet.WriteInteger(MaxLevel);
            packet.WriteInteger(Experience);
            packet.WriteInteger(MaxExperience);
            packet.WriteInteger(Energy);
            packet.WriteInteger(MaxEnergy);
            packet.WriteInteger(Happiness);
            packet.WriteInteger(MaxHappiness);
            packet.WriteInteger(Scratches);
            packet.WriteInteger(OwnerId);
            packet.WriteInteger(Age);
            packet.WriteString(OwnerName);
            packet.WriteInteger(Breed);
            packet.WriteBoolean(UnknownBoolA);
            packet.WriteBoolean(UnknownBoolB);

            packet.WriteInteger(UnknownIntsA.Count);
            foreach (int value in UnknownIntsA)
                packet.WriteInteger(value);

            packet.WriteInteger(UnknownIntC);
            packet.WriteBoolean(UnknownBoolC);
            packet.WriteBoolean(UnknownBoolD);
            packet.WriteBoolean(UnknownBoolE);
            packet.WriteInteger(RarityLevel);
            packet.WriteInteger(MaxRemainingWellbeing);
            packet.WriteInteger(RemainingWellbeing);
            packet.WriteInteger(TimeUntilGrownUp);
            packet.WriteBoolean(UnknownBoolF);
        }
    }
}
