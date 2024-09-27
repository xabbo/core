using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="ICrackableFurniData"/>
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
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        Value = p.ReadString();
        Hits = p.ReadInt();
        Target = p.ReadInt();

        base.Initialize(p);
    }

    protected override void WriteData(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        p.WriteString(Value);
        p.WriteInt(Hits);
        p.WriteInt(Target);

        WriteBase(p);
    }
}
