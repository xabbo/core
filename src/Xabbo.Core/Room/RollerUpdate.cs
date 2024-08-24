using System;
using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class RollerUpdate : IComposer, IParser<RollerUpdate>
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

    protected RollerUpdate(in PacketReader p)
        : this()
    {
        LocationX = p.Read<int>();
        LocationY = p.Read<int>();
        TargetX = p.Read<int>();
        TargetY = p.Read<int>();

        int n = p.Read<Length>();
        ObjectUpdates = new List<RollerObjectUpdate>(n);
        for (int i = 0; i < n; i++)
            ObjectUpdates.Add(p.Parse<RollerObjectUpdate>());

        RollerId = p.Read<Id>();

        if (p.Available > 0)
        {
            Type = (RollerUpdateType)p.Read<int>();
            if (Type == RollerUpdateType.MovingEntity ||
                Type == RollerUpdateType.StationaryEntity)
            {
                if (p.Client == ClientType.Unity)
                {
                    p.Read<int>();
                }
                EntityIndex = p.Read<int>();
                EntityLocationZ = p.Read<float>();
                EntityTargetZ = p.Read<float>();
            }
        }
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(LocationX);
        p.Write(LocationY);
        p.Write(TargetX);
        p.Write(TargetY);

        // TODO remove this
        // packet.Write<Length>(ObjectUpdates.Count);
        // foreach (var update in ObjectUpdates)
        //     packet.Write(update);
        p.Write(ObjectUpdates);

        p.Write(RollerId);

        if (Type != RollerUpdateType.None)
        {
            p.Write((int)Type);
            p.Write(EntityIndex);
            p.Write(EntityLocationZ);
            p.Write(EntityTargetZ);
        }
    }

    public static RollerUpdate Parse(in PacketReader p) => new(in p);
}
