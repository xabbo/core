using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IPet"/>
public class Pet(Id id, int index) : Avatar(AvatarType.Pet, id, index), IPet
{
    public int Breed { get; set; }
    public Id OwnerId { get; set; } = -1;
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

    internal Pet(Id id, int index, in PacketReader p)
        : this(id, index)
    {
        if (p.Client == ClientType.Shockwave)
            return;

        Breed = p.ReadInt();
        OwnerId = p.ReadId();
        OwnerName = p.ReadString();
        RarityLevel = p.ReadInt();
        HasSaddle = p.ReadBool();
        IsRiding = p.ReadBool();
        CanBreed = p.ReadBool();
        CanHarvest = p.ReadBool();
        CanRevive = p.ReadBool();
        HasBreedingPermission = p.ReadBool();
        Level = p.ReadInt();
        Posture = p.ReadString();
    }

    public override void Compose(in PacketWriter p)
    {
        base.Compose(in p);

        p.WriteInt(Breed);
        p.WriteId(OwnerId);
        p.WriteString(OwnerName);
        p.WriteInt(RarityLevel);
        p.WriteBool(HasSaddle);
        p.WriteBool(IsRiding);
        p.WriteBool(CanBreed);
        p.WriteBool(CanHarvest);
        p.WriteBool(CanRevive);
        p.WriteBool(HasBreedingPermission);
        p.WriteInt(Level);
        p.WriteString(Posture);
    }
}
