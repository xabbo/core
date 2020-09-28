using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public abstract class Furni : IFurni
    {
        public abstract ItemType Type { get; }

        public bool IsFloorItem => Type == ItemType.Floor;
        public bool IsWallItem => Type == ItemType.Wall;

        public int Kind { get; set; }
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }

        public int State { get; set; }

        public int SecondsToExpiration { get; set; }
        public FurniUsage Usage { get; set; }

        public abstract void Write(Packet packet);
        public abstract void Write(Packet packet, bool writeName = true);
    }
}
