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
        Value = p.ReadString();
        Result = p.ReadInt();

        base.Initialize(in p);
    }

    protected override void WriteData(in PacketWriter p)
    {
        p.WriteString(Value);
        p.WriteInt(Result);

        WriteBase(in p);
    }
}
