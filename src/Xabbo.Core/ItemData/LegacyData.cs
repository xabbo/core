using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="ILegacyData"/>
public class LegacyData : ItemData, ILegacyData
{
    public LegacyData()
        : base(ItemDataType.Legacy)
    { }

    public LegacyData(ILegacyData data)
        : base(data)
    {
        Flags = data.Flags;
        Value = data.Value;
    }

    protected override void Initialize(in PacketReader p)
    {
        Value = p.ReadString();
        base.Initialize(p);
    }

    protected override void WriteData(in PacketWriter p)
    {
        p.WriteString(Value);
        WriteBase(p);
    }
}
