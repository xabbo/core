using System;
using Xabbo.Messages;

namespace Xabbo.Core
{
    public abstract class ItemData : IItemData
    {
        public ItemDataType Type { get; }

        public ItemDataFlags Flags { get; set; }

        public bool IsLimitedRare => Flags.HasFlag(ItemDataFlags.IsLimitedRare);

        public int UniqueSerialNumber { get; set; }
        public int UniqueSeriesSize { get; set; }

        public string Value { get; set; }

        public int State => int.TryParse(Value, out int state) ? state : -1;

        protected ItemData(ItemDataType type)
        {
            Type = type;
            Value = string.Empty;
        }

        protected ItemData(IItemData data)
        {
            Type = data.Type;
            Value = data.Value;
            UniqueSerialNumber = data.UniqueSerialNumber;
            UniqueSeriesSize = data.UniqueSeriesSize;
        }

        protected virtual void Initialize(IReadOnlyPacket packet)
        {
            if (Flags.HasFlag(ItemDataFlags.IsLimitedRare))
            {
                UniqueSerialNumber = packet.ReadInt();
                UniqueSeriesSize = packet.ReadInt();
            }
        }

        public void Compose(IPacket packet)
        {
            packet.WriteInt(((int)Type & 0xFF) | ((int)Flags << 8));
            WriteData(packet);
        }

        protected void WriteBase(IPacket packet)
        {
            if (Flags.HasFlag(ItemDataFlags.IsLimitedRare))
            {
                packet.WriteInt(UniqueSerialNumber);
                packet.WriteInt(UniqueSeriesSize);
            }
        }

        protected abstract void WriteData(IPacket packet);

        public static ItemData Clone(IItemData data)
        {
            return data switch
            {
                ILegacyData x => new LegacyData(x),
                IMapData x => new MapData(x),
                IStringArrayData x => new StringArrayData(x),
                IVoteResultData x => new VoteResultData(x),
                IEmptyItemData x => new EmptyItemData(x),
                IIntArrayData x => new IntArrayData(x),
                IHighScoreData x => new HighScoreData(x),
                ICrackableFurniData x => new CrackableFurniData(x),
                _ => throw new Exception($"Unknown item data type: {data.Type}."),
            };
        }

        public static ItemData Parse(IReadOnlyPacket packet)
        {
            int value = packet.ReadInt();
            var type = (ItemDataType)(value & 0xFF);

            ItemData data = type switch
            {
                ItemDataType.Legacy => new LegacyData(),
                ItemDataType.Map => new MapData(),
                ItemDataType.StringArray => new StringArrayData(),
                ItemDataType.VoteResult => new VoteResultData(),
                ItemDataType.Empty => new EmptyItemData(),
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
