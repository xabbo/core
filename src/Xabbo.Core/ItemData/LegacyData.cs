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

    protected override void Initialize(in PacketReader packet)
    {
        Value = packet.Read<string>();
        base.Initialize(packet);
    }

    protected override void WriteData(in PacketWriter packet)
    {
        packet.Write(Value);
        WriteBase(packet);
    }
}
