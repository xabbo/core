using Xabbo.Messages;

namespace Xabbo.Core;

public class Pet(long id, int index) : Entity(EntityType.Pet, id, index), IPet
{
    public int Breed { get; set; }
    public long OwnerId { get; set; } = -1;
    public string OwnerName { get; set; } = "(unknown)";
    public int RarityLevel { get; set; }
    public bool HasSaddle { get; set; }
    public bool IsRiding { get; set; }
    public bool CanBreed { get; set; }
    public bool CanHarvest { get; set; }
    public bool CanRevive { get; set; }
    public bool HasBreedingPermission { get; set; }
    public int Level { get; set; }
    public string Posture { get; set; } = "";

    internal Pet(long id, int index, in PacketReader p)
        : this(id, index)
    {
        Breed = p.Read<int>();
        OwnerId = p.Read<Id>();
        OwnerName = p.Read<string>();
        RarityLevel = p.Read<int>();
        HasSaddle = p.Read<bool>();
        IsRiding = p.Read<bool>();
        CanBreed = p.Read<bool>();
        CanHarvest = p.Read<bool>();
        CanRevive = p.Read<bool>();
        HasBreedingPermission = p.Read<bool>();
        Level = p.Read<int>();
        Posture = p.Read<string>();
    }

    public override void Compose(in PacketWriter p)
    {
        base.Compose(in p);

        p.Write(Breed);
        p.Write(OwnerId);
        p.Write(RarityLevel);
        p.Write(HasSaddle);
        p.Write(IsRiding);
        p.Write(CanBreed);
        p.Write(CanHarvest);
        p.Write(CanRevive);
        p.Write(HasBreedingPermission);
        p.Write(Level);
        p.Write(Posture);
    }
}
