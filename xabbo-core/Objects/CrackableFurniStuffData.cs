using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class CrackableFurniStuffData : StuffData
    {
        public int Hits { get; private set; }
        public int Target { get; private set; }

        public CrackableFurniStuffData()
            : base(StuffDataType.CrackableFurniStuffData)
        { }

        protected override void Initialize(Packet packet)
        {
            LegacyString = packet.ReadString();
            Hits = packet.ReadInteger();
            Target = packet.ReadInteger();

            base.Initialize(packet);
        }

        protected override void WriteData(Packet packet)
        {
            packet.WriteString(LegacyString);
            packet.WriteInteger(Hits);
            packet.WriteInteger(Target);

            WriteBase(packet);
        }
    }
}
