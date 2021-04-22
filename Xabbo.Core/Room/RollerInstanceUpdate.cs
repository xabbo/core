using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class RollerObjectUpdate : IComposable
    {
        public long Id { get; set; }
        public float LocationZ { get; set; }
        public float TargetZ { get; set; }

        public RollerObjectUpdate() { }

        protected RollerObjectUpdate(IReadOnlyPacket packet)
        {
            Id = packet.ReadLegacyLong();
            LocationZ = packet.ReadLegacyFloat();
            TargetZ = packet.ReadLegacyFloat();
        }

        public void Compose(IPacket packet) => packet
            .WriteLegacyLong(Id)
            .WriteLegacyFloat(LocationZ)
            .WriteLegacyFloat(TargetZ);

        public static RollerObjectUpdate Parse(IReadOnlyPacket packet) => new RollerObjectUpdate(packet);
    }
}
