using Xabbo.Messages;
using Xabbo.Core.Extensions;

namespace Xabbo.Core;

public abstract class Furni : IFurni
{
    public abstract ItemType Type { get; }

    public bool IsFloorItem => Type == ItemType.Floor;
    public bool IsWallItem => Type == ItemType.Wall;

    public int Kind { get; set; }
    public long Id { get; set; }
    public long OwnerId { get; set; }
    public string OwnerName { get; set; }

    public abstract int State { get; }

    public int SecondsToExpiration { get; set; }
    public FurniUsage Usage { get; set; }

    public bool IsHidden { get; set; }

    public abstract void Compose(IPacket packet);
    public abstract void Compose(IPacket packet, bool writeName = true);

    public Furni()
    {
        OwnerName = string.Empty;
    }
}
