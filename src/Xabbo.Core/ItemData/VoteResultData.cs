using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public class VoteResultData : ItemData, IVoteResultData
{
    public int Result { get; set; }

    public VoteResultData()
        : base(ItemDataType.VoteResult)
    { }

    public VoteResultData(IVoteResultData data)
        : base(data)
    {
        Result = data.Result;
    }

    protected override void Initialize(in PacketReader packet)
    {
        Value = packet.Read<string>();
        Result = packet.Read<int>();

        base.Initialize(packet);
    }

    protected override void WriteData(in PacketWriter packet)
    {
        packet.Write(Value);
        packet.Write(Result);

        WriteBase(packet);
    }
}
