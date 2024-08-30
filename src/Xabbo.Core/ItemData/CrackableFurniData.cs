using Xabbo.Messages;

namespace Xabbo.Core;

public class CrackableFurniData : ItemData, ICrackableFurniData
{
    public int Hits { get; set; }
    public int Target { get; set; }

    public CrackableFurniData()
        : base(ItemDataType.CrackableFurni)
    { }

    public CrackableFurniData(ICrackableFurniData data)
        : base(data)
    {
        Hits = data.Hits;
        Target = data.Target;
    }

    protected override void Initialize(in PacketReader p)
    {
        Value = p.ReadString();
        Hits = p.ReadInt();
        Target = p.ReadInt();

        base.Initialize(p);
    }

    protected override void WriteData(in PacketWriter p)
    {
        p.WriteString(Value);
        p.WriteInt(Hits);
        p.WriteInt(Target);

        WriteBase(p);
    }
}
