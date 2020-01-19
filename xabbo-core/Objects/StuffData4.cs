using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class StuffData4 : StuffData
    {
        public StuffData4()
            : base(StuffDataType.StuffData4)
        { }

        protected override void WriteData(Packet packet) { }
    }
}
