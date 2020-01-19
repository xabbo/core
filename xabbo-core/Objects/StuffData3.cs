using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class StuffData3 : StuffData
    {
        public int Result { get; private set; }

        public StuffData3()
            : base(StuffDataType.StuffData3)
        { }

        protected override void Initialize(Packet packet)
        {
            LegacyString = packet.ReadString();
            Result = packet.ReadInteger();

            base.Initialize(packet);
        }

        protected override void WriteData(Packet packet)
        {
            packet.WriteString(LegacyString);
            packet.WriteInteger(Result);

            WriteBase(packet);
        }
    }
}
