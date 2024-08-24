using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public class RollerObjectUpdate : IComposer, IParser<RollerObjectUpdate>
{
    public long Id { get; set; }
    public float LocationZ { get; set; }
    public float TargetZ { get; set; }

    public RollerObjectUpdate() { }

    protected RollerObjectUpdate(in PacketReader p)
    {
        Id = p.Read<Id>();
        LocationZ = p.Read<float>();
        TargetZ = p.Read<float>();
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(Id);
        p.Write(LocationZ);
        p.Write(TargetZ);
    }

    public static RollerObjectUpdate Parse(in PacketReader p) => new(p);
}
