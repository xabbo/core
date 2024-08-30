using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public class RollerUpdate : IParserComposer<RollerUpdate>
{
    public int LocationX { get; set; }
    public int LocationY { get; set; }
    public int TargetX { get; set; }
    public int TargetY { get; set; }
    public List<RollerObjectUpdate> ObjectUpdates { get; set; }
    public Id RollerId { get; set; }
    public RollerUpdateType Type { get; set; }
    public int EntityIndex { get; set; }
    public float EntityLocationZ { get; set; }
    public float EntityTargetZ { get; set; }

    public RollerUpdate()
    {
        ObjectUpdates = [];
        Type = RollerUpdateType.None;
    }

    protected RollerUpdate(in PacketReader p)
    {
        LocationX = p.ReadInt();
        LocationY = p.ReadInt();
        TargetX = p.ReadInt();
        TargetY = p.ReadInt();

        int n = p.ReadLength();
        ObjectUpdates = new List<RollerObjectUpdate>(n);
        for (int i = 0; i < n; i++)
            ObjectUpdates.Add(p.Parse<RollerObjectUpdate>());

        RollerId = p.ReadId();

        Type = RollerUpdateType.None;
        if (p.Available > 0)
        {
            Type = (RollerUpdateType)p.ReadInt();
            if (Type == RollerUpdateType.MovingEntity ||
                Type == RollerUpdateType.StationaryEntity)
            {
                if (p.Client == ClientType.Unity)
                {
                    p.ReadInt();
                }
                EntityIndex = p.ReadInt();
                EntityLocationZ = p.ReadFloat();
                EntityTargetZ = p.ReadFloat();
            }
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(LocationX);
        p.WriteInt(LocationY);
        p.WriteInt(TargetX);
        p.WriteInt(TargetY);

        p.ComposeArray(ObjectUpdates);

        p.WriteId(RollerId);

        if (Type != RollerUpdateType.None)
        {
            p.WriteInt((int)Type);
            p.WriteInt(EntityIndex);
            p.WriteFloat(EntityLocationZ);
            p.WriteFloat(EntityTargetZ);
        }
    }

    static RollerUpdate IParser<RollerUpdate>.Parse(in PacketReader p) => new(in p);
}
