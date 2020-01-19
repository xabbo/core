using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class StuffData0 : StuffData
    {
        public StuffData0()
            : base(StuffDataType.StuffData0)
        { }

        protected override void Initialize(Packet packet)
        {
            LegacyString = packet.ReadString();
            base.Initialize(packet);
        }

        protected override void WriteData(Packet packet)
        {
            packet.WriteString(LegacyString);
            WriteBase(packet);
        }
    }
}
