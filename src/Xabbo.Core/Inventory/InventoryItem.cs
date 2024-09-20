using System;
using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class InventoryItem : IInventoryItem, IParserComposer<InventoryItem>
{
    public Id ItemId { get; set; }
    public ItemType Type { get; set; }
    public Id Id { get; set; }
    public int Kind { get; set; }
    public string Identifier { get; set; } = "";
    public FurniCategory Category { get; set; }
    public ItemData Data { get; set; }
    IItemData IInventoryItem.Data => Data;
    public bool IsRecyclable { get; set; }
    public bool IsTradeable { get; set; }
    public bool IsGroupable { get; set; }
    public bool IsSellable { get; set; }
    public int SecondsToExpiration { get; set; }
    public bool HasRentPeriodStarted { get; set; }
    public Id RoomId { get; set; }
    public short _Short1 { get; set; }
    public string SlotId { get; set; } = "";
    public int _Int3 { get; set; }

    public string _String3 { get; set; } = "";
    public long Extra { get; set; }
    public int _Int5 { get; set; }

    public bool IsFloorItem => Type == ItemType.Floor;
    public bool IsWallItem => Type == ItemType.Wall;

    public InventoryItem()
    {
        Data = new LegacyData();
    }

    public InventoryItem(IInventoryItem item)
    {
        Type = item.Type;
        Kind = item.Kind;

        Id = item.Id;
        ItemId = item.ItemId;
        Category = item.Category;
        Data = (ItemData)item.Data; // TODO Deep copy data
    }

    private InventoryItem(in PacketReader p) : this()
    {
        switch (p.Client)
        {
            case ClientType.Unity or ClientType.Flash:
                ParseModern(in p);
                break;
            case ClientType.Shockwave:
                ParseOrigins(in p);
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }

    private void ParseModern(in PacketReader p)
    {
        ItemId = p.ReadId();

        if (p.Client == ClientType.Flash)
        {
            Type = H.ToItemType(p.ReadString());
        }
        else
        {
            Type = H.ToItemType(p.ReadShort());
        }

        Id = p.ReadId();
        Kind = p.ReadInt();
        Category = (FurniCategory)p.ReadInt();
        Data = p.Parse<ItemData>();
        IsRecyclable = p.ReadBool();
        IsTradeable = p.ReadBool();
        IsGroupable = p.ReadBool();
        IsSellable = p.ReadBool();
        SecondsToExpiration = p.ReadInt();
        HasRentPeriodStarted = p.ReadBool();
        RoomId = p.ReadId();

        if (p.Client == ClientType.Unity)
        {
            // - Seems to be consistent
            _Short1 = p.ReadShort(); // ?
            SlotId = p.ReadString(); // string "r" / "s"
            _Int3 = p.ReadInt(); // int 1187551480
        }

        if (Type == ItemType.Floor)
        {
            if (p.Client == ClientType.Flash)
            {
                SlotId = p.ReadString();
                Extra = p.ReadInt();
            }
            else
            {
                // 10 bytes ?
                _String3 = p.ReadString();
                Extra = p.ReadInt();
                _Int5 = p.ReadInt();
            }
        }
        else
        {
            _String3 = "";
        }
    }

    private void ParseOrigins(in PacketReader p)
    {
        Kind = -1;
        ItemId = p.ReadInt();
        SlotId = p.ReadInt().ToString();
        string strItemType = p.ReadString();
        Type = strItemType switch
        {
            "S" => ItemType.Floor,
            "I" => ItemType.Wall,
            _ => throw new Exception($"Invalid item type: {strItemType}"),
        };
        Id = p.ReadInt();
        Identifier = p.ReadString();
        if (Type == ItemType.Floor)
        {
            p.ReadInt(); // dimX
            p.ReadInt(); // dimY
            // colors
            Data = new LegacyData { Value = p.ReadString() };
        }
        else if (Type == ItemType.Wall)
        {
            // props
            Data = new LegacyData { Value = p.ReadString() };
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        // TODO origins composer
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.WriteId(ItemId);

        if (p.Client == ClientType.Flash)
        {
            p.WriteString(Type.ToShortString().ToUpper());
        }
        else
        {
            p.WriteShort(Type.GetValue());
        }

        p.WriteId(Id);
        p.WriteInt(Kind);
        p.WriteInt((int)Category);
        p.Compose(Data);
        p.WriteBool(IsRecyclable);
        p.WriteBool(IsTradeable);
        p.WriteBool(IsGroupable);
        p.WriteBool(IsSellable);
        p.WriteInt(SecondsToExpiration);
        p.WriteBool(HasRentPeriodStarted);
        p.WriteId(RoomId);

        if (p.Client == ClientType.Unity)
        {
            p.WriteShort(_Short1);
            p.WriteString(SlotId);
            p.WriteInt(_Int3);
        }

        if (Type == ItemType.Floor)
        {
            if (p.Client == ClientType.Flash)
            {
                p.WriteString(SlotId);
                p.WriteInt((int)Extra);
            }
            else
            {
                // 10 bytes ?
                p.WriteString(_String3);
                p.WriteLong(Extra);
                p.WriteInt(_Int5);
            }
        }
    }

    static InventoryItem IParser<InventoryItem>.Parse(in PacketReader p) => new(in p);

    public override string ToString() => $"{nameof(InventoryItem)}#{ItemId}/{Type}:{Kind}";
}
