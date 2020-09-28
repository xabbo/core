using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class RollerUpdate : IWritable
    {
        public static RollerUpdate Parse(Packet packet) => new RollerUpdate(packet);

        public int LocationX { get; set; }
        public int LocationY { get; set; }
        public int TargetX { get; set; }
        public int TargetY { get; set; }
        public List<RollerObjectUpdate> ObjectUpdates { get; set; }
        public int RollerId { get; set; }
        public RollerUpdateType Type { get; set; }
        public int EntityIndex { get; set; }
        public double EntityLocationZ { get; set; }
        public double EntityTargetZ { get; set; }

        public RollerUpdate()
        {
            ObjectUpdates = new List<RollerObjectUpdate>();
            Type = RollerUpdateType.None;
        }

        protected RollerUpdate(Packet packet)
            : this()
        {
            LocationX = packet.ReadInt();
            LocationY = packet.ReadInt();
            TargetX = packet.ReadInt();
            TargetY = packet.ReadInt();
            int n = packet.ReadInt();
            for (int i = 0; i < n; i++)
                ObjectUpdates.Add(RollerObjectUpdate.Parse(packet));
            RollerId = packet.ReadInt();

            if (packet.Available > 0)
            {
                Type = (RollerUpdateType)packet.ReadInt();
                if (Type == RollerUpdateType.MovingEntity ||
                    Type == RollerUpdateType.StationaryEntity)
                {
                    EntityIndex = packet.ReadInt();
                    EntityLocationZ = packet.ReadDouble();
                    EntityTargetZ = packet.ReadDouble();
                }
            }
        }

        public void Write(Packet packet)
        {
            packet.WriteValues(
                LocationX,
                LocationY,
                TargetX,
                TargetY,
                ObjectUpdates,
                RollerId
            );

            if (Type != RollerUpdateType.None)
            {
                packet.WriteValues((int)Type, EntityIndex, EntityLocationZ, EntityTargetZ);
            }
        }
    }
}
