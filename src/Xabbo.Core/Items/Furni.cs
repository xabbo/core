using Xabbo.Messages;

namespace Xabbo.Core;

public abstract class Furni : IFurni
{
    public abstract ItemType Type { get; }

    public bool IsFloorItem => Type == ItemType.Floor;
    public bool IsWallItem => Type == ItemType.Wall;

    public int Kind { get; set; }
    public Id Id { get; set; }
    public Id OwnerId { get; set; }
    public string OwnerName { get; set; }

    public abstract int State { get; }

    public int SecondsToExpiration { get; set; }
    public FurniUsage Usage { get; set; }

    public bool IsHidden { get; set; }

    public Furni()
    {
        OwnerName = string.Empty;
    }

    public abstract void Compose(in PacketWriter p);
    public abstract void Compose(in PacketWriter p, bool writeOwnerName);
}
