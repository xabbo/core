using System;
using System.Collections.Generic;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class IntArrayStuffData : StuffData
    {
        private List<int> array;
        public IReadOnlyList<int> Values { get; }

        public IntArrayStuffData()
            : base(StuffDataType.IntArrayStuffData)
        {
            array = new List<int>();
            Values = array.AsReadOnly();
        }

        protected override void Initialize(Packet packet)
        {
            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                array.Add(packet.ReadInteger());

            base.Initialize(packet);
        }

        protected override void WriteData(Packet packet)
        {
            packet.WriteInteger(Values.Count);
            foreach (int value in Values)
                packet.WriteInteger(value);

            WriteBase(packet);
        }
    }
}
