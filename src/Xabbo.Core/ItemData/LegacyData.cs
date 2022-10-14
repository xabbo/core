using System;

using Xabbo.Messages;

namespace Xabbo.Core;

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

    protected override void Initialize(IReadOnlyPacket packet)
    {
        Value = packet.ReadString();
        base.Initialize(packet);
    }

    protected override void WriteData(IPacket packet)
    {
        packet.WriteString(Value);
        WriteBase(packet);
    }
}
