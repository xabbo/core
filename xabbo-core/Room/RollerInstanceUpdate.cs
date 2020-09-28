using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class RollerObjectUpdate : IWritable
    {
        public static RollerObjectUpdate Parse(Packet packet) => new RollerObjectUpdate(packet);

        public int Id { get; set; }
        public double LocationZ { get; set; }
        public double TargetZ { get; set; }

        public RollerObjectUpdate() { }

        protected RollerObjectUpdate(Packet packet)
        {
            Id = packet.ReadInt();
            LocationZ = packet.ReadDouble();
            TargetZ = packet.ReadDouble();
        }

        public void Write(Packet packet) => packet.WriteValues(
            Id,
            LocationZ,
            TargetZ
        );
    }
}
