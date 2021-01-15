using System;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public abstract class StuffData : IItemData
    {
        public StuffDataType Type { get; }

        public ItemDataFlags Flags { get; set; }

        public int LimitedNumber { get; set; }
        public int LimitedTotal { get; set; }

        public string Value { get; set; }

        public int State => int.TryParse(Value, out int state) ? state : -1;

        protected StuffData(StuffDataType type)
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

        public static StuffData Parse(IReadOnlyPacket packet)
        {
            int value = packet.ReadInt();
            var type = (StuffDataType)(value & 0xFF);

            StuffData data = type switch
            {
                StuffDataType.Legacy => new LegacyData(),
                StuffDataType.Map => new MapData(),
                StuffDataType.StringArray => new StringArrayData(),
                StuffDataType.VoteResult => new VoteResultData(),
                StuffDataType.ItemData4 => new ItemData4(),
                StuffDataType.IntArray => new IntArrayData(),
                StuffDataType.HighScore => new HighScoreData(),
                StuffDataType.CrackableFurni => new CrackableFurniData(),
                _ => throw new Exception($"Unknown ItemData type: {type}"),
            };

            data.Flags = (ItemDataFlags)(value >> 8);
            data.Initialize(packet);

            return data;
        }
    }
}
