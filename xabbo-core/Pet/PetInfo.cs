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
			IntA = packet.ReadInt();
			BoolA = packet.ReadBool();
			BoolB = packet.ReadBool();

			int n = packet.ReadInt();
			for (int i = 0; i < n; i++)
				IntsA.Add(packet.ReadInt());

			IntB = packet.ReadInt();
			BoolC = packet.ReadBool();
			BoolD = packet.ReadBool();
			BoolE = packet.ReadBool();
			RarityLevel = packet.ReadInt();
			MaxWellbeing = packet.ReadInt();
			Wellbeing = packet.ReadInt();
			IntC = packet.ReadInt();
			BoolF = packet.ReadBool();
		}

		public void Write(Packet packet)
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
			packet.WriteInt(IntA);
			packet.WriteBool(BoolA);
			packet.WriteBool(BoolB);

			packet.WriteInt(IntsA.Count);
			foreach (int value in IntsA)
				packet.WriteInt(value);

			packet.WriteInt(IntB);
			packet.WriteBool(BoolC);
			packet.WriteBool(BoolD);
			packet.WriteBool(BoolE);
			packet.WriteInt(RarityLevel);
			packet.WriteInt(MaxWellbeing);
			packet.WriteInt(Wellbeing);
			packet.WriteInt(IntC);
			packet.WriteBool(BoolF);
		}
	}
}
