using Xabbo.Messages;

namespace Xabbo.Core;

public class RollerObjectUpdate : IParserComposer<RollerObjectUpdate>
{
    public long Id { get; set; }
    public float LocationZ { get; set; }
    public float TargetZ { get; set; }

    public RollerObjectUpdate() { }

    protected RollerObjectUpdate(in PacketReader p)
    {
        Id = p.ReadId();
        LocationZ = p.ReadFloat();
        TargetZ = p.ReadFloat();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteFloat(LocationZ);
        p.WriteFloat(TargetZ);
    }

    static RollerObjectUpdate IParser<RollerObjectUpdate>.Parse(in PacketReader p) => new(p);
}
