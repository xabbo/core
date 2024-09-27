using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IVoteResultData"/>
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
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        Value = p.ReadString();
        Result = p.ReadInt();

        base.Initialize(in p);
    }

    protected override void WriteData(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfNoneOr(p.Client, ClientType.Shockwave);

        p.WriteString(Value);
        p.WriteInt(Result);

        WriteBase(in p);
    }
}
