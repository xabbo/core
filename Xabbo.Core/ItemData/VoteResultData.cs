using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
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

        protected override void Initialize(IReadOnlyPacket packet)
        {
            Value = packet.ReadString();
            Result = packet.ReadInt();

            base.Initialize(packet);
        }

        protected override void WriteData(IPacket packet)
        {
            packet.WriteString(Value);
            packet.WriteInt(Result);

            WriteBase(packet);
        }
    }
}
