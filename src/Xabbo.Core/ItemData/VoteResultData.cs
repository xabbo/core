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

    protected override void Initialize(in PacketReader p)
    {
        Value = p.Read<string>();
        Result = p.Read<int>();

        base.Initialize(p);
    }

    protected override void WriteData(in PacketWriter p)
    {
        p.Write(Value);
        p.Write(Result);

        WriteBase(p);
    }
}
