using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IItemData"/>
public abstract class ItemData : IItemData, IParserComposer<ItemData>
{
    public ItemDataType Type { get; }

    public ItemDataFlags Flags { get; set; }

    public bool IsLimitedRare => Flags.HasFlag(ItemDataFlags.IsLimitedRare);

    public int UniqueSerialNumber { get; set; }
    public int UniqueSeriesSize { get; set; }

    public string Value { get; set; }

    public int State
    {
        get
        {
            return Value switch
            {
                // Compatibility for Shockwave
                // C/O = Closed/Open (Gate)
                "C" or "FALSE" => 0,
                "O" or "TRUE" => 1,
                _ => int.TryParse(Value, out int state) ? state : -1
            };
        }
    }

    protected ItemData(ItemDataType type)
    {
        Type = type;
        Value = "";
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
        if (p.Client is not ClientType.Shockwave &&
            Flags.HasFlag(ItemDataFlags.IsLimitedRare))
        {
            UniqueSerialNumber = p.ReadInt();
            UniqueSeriesSize = p.ReadInt();
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is not ClientType.Shockwave)
            p.WriteInt(((int)Type & 0xFF) | ((int)Flags << 8));
        WriteData(in p);
    }

    protected void WriteBase(in PacketWriter p)
    {
        if (p.Client is not ClientType.Shockwave &&
            Flags.HasFlag(ItemDataFlags.IsLimitedRare))
        {
            p.WriteInt(UniqueSerialNumber);
            p.WriteInt(UniqueSeriesSize);
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

    static ItemData IParser<ItemData>.Parse(in PacketReader p)
    {
        ItemDataType type = ItemDataType.Legacy;
        ItemDataFlags flags = ItemDataFlags.None;

        if (p.Client is not ClientType.Shockwave)
        {
            int value = p.ReadInt();
            type = (ItemDataType)(value & 0xFF);
            flags = (ItemDataFlags)(value >> 8);
        }

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

        data.Flags = flags;
        data.Initialize(in p);

        return data;
    }
}
