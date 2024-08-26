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

    protected override void Initialize(in PacketReader p)
    {
        Value = p.Read<string>();
        base.Initialize(p);
    }

    protected override void WriteData(in PacketWriter p)
    {
        p.Write(Value);
        WriteBase(p);
    }
}
