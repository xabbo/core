using System;
using System.Collections.Generic;

using Xabbo.Common;
using Xabbo.Messages;

namespace Xabbo.Core;

public class RollerUpdate : IComposable
{
    public int LocationX { get; set; }
    public int LocationY { get; set; }
    public int TargetX { get; set; }
    public int TargetY { get; set; }
    public List<RollerObjectUpdate> ObjectUpdates { get; set; }
    public long RollerId { get; set; }
    public RollerUpdateType Type { get; set; }
    public int EntityIndex { get; set; }
    public float EntityLocationZ { get; set; }
    public float EntityTargetZ { get; set; }

    public RollerUpdate()
    {
        ObjectUpdates = new List<RollerObjectUpdate>();
        Type = RollerUpdateType.None;
    }

    protected RollerUpdate(IReadOnlyPacket packet)
        : this()
    {
        LocationX = packet.ReadInt();
        LocationY = packet.ReadInt();
        TargetX = packet.ReadInt();
        TargetY = packet.ReadInt();

        int n = packet.ReadLegacyShort();
        for (int i = 0; i < n; i++)
        {
            ObjectUpdates.Add(RollerObjectUpdate.Parse(packet));
        }

        RollerId = packet.ReadLegacyLong();

        if (packet.Available > 0)
        {
            Type = (RollerUpdateType)packet.ReadInt();
            if (Type == RollerUpdateType.MovingEntity ||
                Type == RollerUpdateType.StationaryEntity)
            {
                if (packet.Protocol == ClientType.Unity)
                {
                    packet.ReadInt();
                }
                // Entity index may have changed to long here
                // but it's int everywhere else ???
                EntityIndex = packet.ReadInt();
                EntityLocationZ = packet.ReadLegacyFloat();
                EntityTargetZ = packet.ReadLegacyFloat();
            }
        }
    }

    public void Compose(IPacket packet)
    {
        packet.WriteInt(LocationX);
        packet.WriteInt(LocationY);
        packet.WriteInt(TargetX);
        packet.WriteInt(TargetY);

        packet.WriteLegacyShort((short)ObjectUpdates.Count);
        foreach (var update in ObjectUpdates)
        {
            update.Compose(packet);
        }

        packet.WriteLegacyLong(RollerId);

        if (Type != RollerUpdateType.None)
        {
            packet.WriteInt((int)Type);
            packet.WriteLegacyLong(EntityIndex);
            packet.WriteFloat(EntityLocationZ); // WriteLegacyFloat ?
            packet.WriteFloat(EntityTargetZ);
        }
    }

    public static RollerUpdate Parse(IReadOnlyPacket packet) => new RollerUpdate(packet);
}
