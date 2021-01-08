using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class BasicData : ItemData, ILegacyData
    {
        public BasicData()
            : base(ItemDataType.Basic)
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
