using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public abstract class Furni : IItem, IWritable
    {
        public abstract FurniType Type { get; }
        public int Kind { get; set; }
        public int Id { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }

        public abstract void Write(Packet packet);
        public abstract void Write(Packet packet, bool writeName = true);
    }
}
