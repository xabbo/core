using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public abstract class ItemData : IItemData, IWritable
    {
        public ItemDataType Type { get; }

        public int Flags { get; set; }

        public int LimitedNumber { get; set; }
        public int LimitedTotal { get; set; }

        public string LegacyString { get; set; }

        public int State => int.TryParse(LegacyString, out int state) ? state : -1;

        protected ItemData(ItemDataType type)
        {
            Type = type;
        }

        protected virtual void Initialize(Packet packet)
        {
            if ((Flags & 256) > 0)
            {
                LimitedNumber = packet.ReadInt();
                LimitedTotal = packet.ReadInt();
            }
        }

        public void Write(Packet packet)
        {
            packet.WriteInt((int)Type | (int)(Flags & 0xFFFFFF00));
            WriteData(packet);
        }

        protected void WriteBase(Packet packet)
        {
            if ((Flags & 256) > 0)
            {
                packet.WriteInt(LimitedNumber);
                packet.WriteInt(LimitedTotal);
            }
        }

        protected abstract void WriteData(Packet packet);

        public static ItemData Parse(Packet packet)
        {
            ItemData stuffData;

            int value = packet.ReadInt();
            var stuffDataType = (ItemDataType)(value & 255);
            switch (stuffDataType)
            {
                case ItemDataType.Legacy: stuffData = new LegacyData(); break;
                case ItemDataType.Map: stuffData = new MapData(); break;
                case ItemDataType.StringArray: stuffData = new StringArrayData(); break;
                case ItemDataType.VoteResult: stuffData = new VoteResultData(); break;
                case ItemDataType.ItemData4: stuffData = new ItemData4(); break;
                case ItemDataType.IntArray: stuffData = new IntArrayData(); break;
                case ItemDataType.HighScore: stuffData = new HighScoreData(); break;
                case ItemDataType.CrackableFurni: stuffData = new CrackableFurniData(); break;
                default: stuffData = null; break;
            }

            if (stuffData != null)
            {
                stuffData.Flags = value & 0xFF00;
                stuffData.Initialize(packet);
            }

            return stuffData;
        }
    }
}
