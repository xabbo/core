using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class VoteResultData : ItemData, IVoteResultData
    {
        public int Result { get; set; }

        public VoteResultData()
            : base(ItemDataType.VoteResult)
        { }

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
