using System;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public class LegacyData : StuffData, ILegacyData
    {
        public LegacyData()
            : base(StuffDataType.Legacy)
        { }

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
}
