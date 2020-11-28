using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public abstract class ItemData : IItemData
    {
        public ItemDataType Type { get; }

        public ItemDataFlags Flags { get; set; }

        public int LimitedNumber { get; set; }
        public int LimitedTotal { get; set; }

        public string Value { get; set; }

        public int State => int.TryParse(Value, out int state) ? state : -1;

        protected ItemData(ItemDataType type)
        {
            Type = type;
            Value = string.Empty;
        }

        protected virtual void Initialize(IReadOnlyPacket packet)
        {
            if (Flags.HasFlag(ItemDataFlags.IsLimitedRare))
            {
                LimitedNumber = packet.ReadInt();
                LimitedTotal = packet.ReadInt();
            }
        }

        public void Write(IPacket packet)
        {
            packet.WriteInt(((int)Type & 0xFF) | ((int)Flags << 8));
            WriteData(packet);
        }

        protected void WriteBase(IPacket packet)
        {
            if (Flags.HasFlag(ItemDataFlags.IsLimitedRare))
            {
                packet.WriteInt(LimitedNumber);
                packet.WriteInt(LimitedTotal);
            }
        }

        protected abstract void WriteData(IPacket packet);

        public static ItemData Parse(IReadOnlyPacket packet)
        {
            ItemData stuffData;

            int value = packet.ReadInt();
            var type = (ItemDataType)(value & 0xFF);
            switch (type)
            {
                case ItemDataType.Basic: stuffData = new BasicData(); break;
                case ItemDataType.Map: stuffData = new MapData(); break;
                case ItemDataType.StringArray: stuffData = new StringArrayData(); break;
                case ItemDataType.VoteResult: stuffData = new VoteResultData(); break;
                case ItemDataType.ItemData4: stuffData = new ItemData4(); break;
                case ItemDataType.IntArray: stuffData = new IntArrayData(); break;
                case ItemDataType.HighScore: stuffData = new HighScoreData(); break;
                case ItemDataType.CrackableFurni: stuffData = new CrackableFurniData(); break;
                default: throw new Exception($"Unknown ItemData type: {type}");
            }

            if (stuffData != null)
            {
                stuffData.Flags = (ItemDataFlags)(value >> 8);
                stuffData.Initialize(packet);
            }

            return stuffData;
        }
    }
}
