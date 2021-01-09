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
            int value = packet.ReadInt();
            var type = (ItemDataType)(value & 0xFF);

            ItemData data = type switch
            {
                ItemDataType.Basic => new BasicData(),
                ItemDataType.Map => new MapData(),
                ItemDataType.StringArray => new StringArrayData(),
                ItemDataType.VoteResult => new VoteResultData(),
                ItemDataType.ItemData4 => new ItemData4(),
                ItemDataType.IntArray => new IntArrayData(),
                ItemDataType.HighScore => new HighScoreData(),
                ItemDataType.CrackableFurni => new CrackableFurniData(),
                _ => throw new Exception($"Unknown ItemData type: {type}"),
            };

            data.Flags = (ItemDataFlags)(value >> 8);
            data.Initialize(packet);

            return data;
        }
    }
}
