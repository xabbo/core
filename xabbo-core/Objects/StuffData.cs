using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public abstract class StuffData : IWritable
    {
        public static StuffData Parse(Packet packet)
        {
            StuffData stuffData;

            int value = packet.ReadInteger();
            var stuffDataType = (StuffDataType)(value & 255);
            switch (stuffDataType)
            {
                case StuffDataType.StuffData0: stuffData = new StuffData0(); break;
                case StuffDataType.MapStuffData: stuffData = new MapStuffData(); break;
                case StuffDataType.StringArrayStuffData: stuffData = new StringArrayStuffData(); break;
                case StuffDataType.StuffData3: stuffData = new StuffData3(); break;
                case StuffDataType.StuffData4: stuffData = new StuffData4(); break;
                case StuffDataType.IntArrayStuffData: stuffData = new IntArrayStuffData(); break;
                case StuffDataType.HighScoreStuffData: stuffData = new HighScoreStuffData(); break;
                case StuffDataType.CrackableFurniStuffData: stuffData = new CrackableFurniStuffData(); break;
                default: stuffData = null; break;
            }

            if (stuffData != null)
            {
                stuffData.Flags = value & 0xFF00;
                stuffData.Initialize(packet);
            }

            return stuffData;
        }

        public StuffDataType Type { get; }

        public int Flags { get; protected set; }
        public int LimitedNumber { get; private set; }
        public int LimitedTotal { get; private set; }

        public string LegacyString { get; protected set; }

        protected StuffData(StuffDataType type)
        {
            Type = type;
        }

        protected virtual void Initialize(Packet packet)
        {
            if ((Flags & 256) > 0)
            {
                LimitedNumber = packet.ReadInteger();
                LimitedTotal = packet.ReadInteger();
            }
        }

        public void Write(Packet packet)
        {
            packet.WriteInteger((int)Type | Flags);
            WriteData(packet);
        }

        protected void WriteBase(Packet packet)
        {
            if ((Flags & 256) > 0)
            {
                packet.WriteInteger(LimitedNumber);
                packet.WriteInteger(LimitedTotal);
            }
        }

        protected abstract void WriteData(Packet packet);
    }
}
