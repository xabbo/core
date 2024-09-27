using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <summary>
/// Defines information about a pet.
/// </summary>
public sealed class PetInfo : IParserComposer<PetInfo>
{
    public Id Id { get; set; }
    public string Name { get; set; } = "";
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
    public string OwnerName { get; set; } = "";
    public int Breed { get; set; }
    public bool HasFreeSaddle { get; set; }
    public bool IsRiding { get; set; }
    public List<int> SkillThresholds { get; set; } // Something to do with skills (horse)
    public int AccessRights { get; set; }
    public bool CanBreed { get; set; }
    public bool CanHarvest { get; set; }
    public bool CanRevive { get; set; }
    public int RarityLevel { get; set; }
    public int MaxWellbeingSeconds { get; set; }
    public int RemainingWellbeingSeconds { get; set; }
    public int RemainingGrowingSeconds { get; set; }
    public bool HasBreedingPermission { get; set; }

    public PetInfo()
    {
        SkillThresholds = [];
    }

    private PetInfo(in PacketReader p) : this()
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        Id = p.ReadId();
        Name = p.ReadString();
        Level = p.ReadInt();
        MaxLevel = p.ReadInt();
        Experience = p.ReadInt();
        MaxExperience = p.ReadInt();
        Energy = p.ReadInt();
        MaxEnergy = p.ReadInt();
        Happiness = p.ReadInt();
        MaxHappiness = p.ReadInt();
        Scratches = p.ReadInt();
        OwnerId = p.ReadInt();
        Age = p.ReadInt();
        OwnerName = p.ReadString();
        Breed = p.ReadInt();
        HasFreeSaddle = p.ReadBool();
        IsRiding = p.ReadBool();
        SkillThresholds = [.. p.ReadIntArray()];
        AccessRights = p.ReadInt();
        CanBreed = p.ReadBool();
        CanHarvest = p.ReadBool();
        CanRevive = p.ReadBool();
        RarityLevel = p.ReadInt();
        MaxWellbeingSeconds = p.ReadInt();
        RemainingWellbeingSeconds = p.ReadInt();
        RemainingGrowingSeconds = p.ReadInt();
        HasBreedingPermission = p.ReadBool();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.WriteId(Id);
        p.WriteString(Name);
        p.WriteInt(Level);
        p.WriteInt(MaxLevel);
        p.WriteInt(Experience);
        p.WriteInt(MaxExperience);
        p.WriteInt(Energy);
        p.WriteInt(MaxEnergy);
        p.WriteInt(Happiness);
        p.WriteInt(MaxHappiness);
        p.WriteInt(Scratches);
        p.WriteInt(OwnerId);
        p.WriteInt(Age);
        p.WriteString(OwnerName);
        p.WriteInt(Breed);
        p.WriteBool(HasFreeSaddle);
        p.WriteBool(IsRiding);
        p.WriteIntArray(SkillThresholds);
        p.WriteInt(AccessRights);
        p.WriteBool(CanBreed);
        p.WriteBool(CanHarvest);
        p.WriteBool(CanRevive);
        p.WriteInt(RarityLevel);
        p.WriteInt(MaxWellbeingSeconds);
        p.WriteInt(RemainingWellbeingSeconds);
        p.WriteInt(RemainingGrowingSeconds);
        p.WriteBool(HasBreedingPermission);
    }

    static PetInfo IParser<PetInfo>.Parse(in PacketReader p) => new(in p);
}
