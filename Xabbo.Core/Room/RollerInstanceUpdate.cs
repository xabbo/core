using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class RollerObjectUpdate : IPacketData
    {
        public static RollerObjectUpdate Parse(IReadOnlyPacket packet) => new RollerObjectUpdate(packet);

        public int Id { get; set; }
        public float LocationZ { get; set; }
        public float TargetZ { get; set; }

        public RollerObjectUpdate() { }

        protected RollerObjectUpdate(IReadOnlyPacket packet)
        {
            Id = packet.ReadInt();
            LocationZ = packet.ReadFloat();
            TargetZ = packet.ReadFloat();
        }

        public void Write(IPacket packet) => packet.WriteValues(
            Id,
            LocationZ,
            TargetZ
        );
    }
}
