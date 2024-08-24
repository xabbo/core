using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class PetInfo : IComposer, IParser<PetInfo>
{
    public Id Id { get; set; }
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

    private PetInfo(in PacketReader p) : this()
    {
        Id = p.Read<Id>();
        Name = p.Read<string>();
        Level = p.Read<int>();
        MaxLevel = p.Read<int>();
        Experience = p.Read<int>();
        MaxExperience = p.Read<int>();
        Energy = p.Read<int>();
        MaxEnergy = p.Read<int>();
        Happiness = p.Read<int>();
        MaxHappiness = p.Read<int>();
        Scratches = p.Read<int>();
        OwnerId = p.Read<int>();
        Age = p.Read<int>();
        OwnerName = p.Read<string>();
        Breed = p.Read<int>();
        HasFreeSaddle = p.Read<bool>();
        IsRiding = p.Read<bool>();

        int n = p.Read<int>();
        for (int i = 0; i < n; i++)
            SkillThresholds.Add(p.Read<int>());

        AccessRights = p.Read<int>();
        CanBreed = p.Read<bool>();
        CanHarvest = p.Read<bool>();
        CanRevive = p.Read<bool>();
        RarityLevel = p.Read<int>();
        MaxWellbeingSeconds = p.Read<int>();
        RemainingWellbeingSeconds = p.Read<int>();
        RemainingGrowingSeconds = p.Read<int>();
        HasBreedingPermission = p.Read<bool>();
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write(Name);
        p.Write(Level);
        p.Write(MaxLevel);
        p.Write(Experience);
        p.Write(MaxExperience);
        p.Write(Energy);
        p.Write(MaxEnergy);
        p.Write(Happiness);
        p.Write(MaxHappiness);
        p.Write(Scratches);
        p.Write(OwnerId);
        p.Write(Age);
        p.Write(OwnerName);
        p.Write(Breed);
        p.Write(HasFreeSaddle);
        p.Write(IsRiding);

        p.Write(SkillThresholds.Count);
        foreach (int value in SkillThresholds)
            p.Write(value);

        p.Write(AccessRights);
        p.Write(CanBreed);
        p.Write(CanHarvest);
        p.Write(CanRevive);
        p.Write(RarityLevel);
        p.Write(MaxWellbeingSeconds);
        p.Write(RemainingWellbeingSeconds);
        p.Write(RemainingGrowingSeconds);
        p.Write(HasBreedingPermission);
    }

    public static PetInfo Parse(in PacketReader packet) => new(packet);
}
