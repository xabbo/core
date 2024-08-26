﻿using Xabbo.Messages;

namespace Xabbo.Core;

public abstract class Furni : IFurni
{
    public abstract ItemType Type { get; }

    public bool IsFloorItem => Type == ItemType.Floor;
    public bool IsWallItem => Type == ItemType.Wall;

    public int Kind { get; set; }
    public Id Id { get; set; }
    public Id OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;

    public abstract int State { get; }

    public int SecondsToExpiration { get; set; } = -1;
    public FurniUsage Usage { get; set; } = FurniUsage.None;

    public string Identifier { get; set; } = string.Empty;

    public bool IsHidden { get; set; }

    public abstract void Compose(in PacketWriter p);
    public abstract void Compose(in PacketWriter p, bool writeOwnerName);
}
