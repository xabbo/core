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
		public int IntA { get; set; }
		public bool BoolA { get; set; }
		public bool BoolB { get; set; }
		public List<int> IntsA { get; set; }
		public int IntB { get; set; }
		public bool BoolC { get; set; }
		public bool BoolD { get; set; }
		public bool BoolE { get; set; }
		public int RarityLevel { get; set; }
		public int MaxWellbeing { get; set; }
		public int Wellbeing { get; set; }
		public int IntC { get; set; }
		public bool BoolF { get; set; }

		public PetInfo()
		{
			IntsA = new List<int>();
		}

		internal PetInfo(Packet packet)
			: base()
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
			IntA = packet.ReadInteger();
			BoolA = packet.ReadBoolean();
			BoolB = packet.ReadBoolean();

			int n = packet.ReadInteger();
			for (int i = 0; i < n; i++)
				IntsA.Add(packet.ReadInteger());

			IntB = packet.ReadInteger();
			BoolC = packet.ReadBoolean();
			BoolD = packet.ReadBoolean();
			BoolE = packet.ReadBoolean();
			RarityLevel = packet.ReadInteger();
			MaxWellbeing = packet.ReadInteger();
			Wellbeing = packet.ReadInteger();
			IntC = packet.ReadInteger();
			BoolF = packet.ReadBoolean();
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
			packet.WriteInteger(IntA);
			packet.WriteBoolean(BoolA);
			packet.WriteBoolean(BoolB);

			packet.WriteInteger(IntsA.Count);
			foreach (int value in IntsA)
				packet.WriteInteger(value);

			packet.WriteInteger(IntB);
			packet.WriteBoolean(BoolC);
			packet.WriteBoolean(BoolD);
			packet.WriteBoolean(BoolE);
			packet.WriteInteger(RarityLevel);
			packet.WriteInteger(MaxWellbeing);
			packet.WriteInteger(Wellbeing);
			packet.WriteInteger(IntC);
			packet.WriteBoolean(BoolF);
		}
	}
}
