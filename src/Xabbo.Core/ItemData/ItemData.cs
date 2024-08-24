using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public abstract class ItemData : IItemData, IComposer, IParser<ItemData>
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

    protected virtual void Initialize(in PacketReader p)
    {
        if (Flags.HasFlag(ItemDataFlags.IsLimitedRare))
        {
            UniqueSerialNumber = p.Read<int>();
            UniqueSeriesSize = p.Read<int>();
        }
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(((int)Type & 0xFF) | ((int)Flags << 8));
        WriteData(in p);
    }

    protected void WriteBase(in PacketWriter p)
    {
        if (Flags.HasFlag(ItemDataFlags.IsLimitedRare))
        {
            p.Write(UniqueSerialNumber);
            p.Write(UniqueSeriesSize);
        }
    }

    protected abstract void WriteData(in PacketWriter p);

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

    public static ItemData Parse(in PacketReader p)
    {
        int value = p.Read<int>();
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
        data.Initialize(in p);

        return data;
    }
}
