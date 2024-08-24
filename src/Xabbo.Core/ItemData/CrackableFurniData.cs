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
        Value = p.Read<string>();
        Hits = p.Read<int>();
        Target = p.Read<int>();

        base.Initialize(p);
    }

    protected override void WriteData(in PacketWriter p)
    {
        p.Write(Value);
        p.Write(Hits);
        p.Write(Target);

        WriteBase(p);
    }
}
