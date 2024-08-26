using System;
using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class InventoryItem : IInventoryItem, IComposer, IParser<InventoryItem>
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
    public long RoomId { get; set; }
    public short _Short1 { get; set; }
    public string SlotId { get; set; } = string.Empty;
    public int _Int3 { get; set; }

    public string _String3 { get; set; } = string.Empty;
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
        ItemId = p.Read<Id>();

        if (p.Client == ClientType.Flash)
        {
            Type = H.ToItemType(p.Read<string>());
        }
        else
        {
            Type = H.ToItemType(p.Read<short>());
        }

        Id = p.Read<Id>();
        Kind = p.Read<int>();
        Category = (FurniCategory)p.Read<int>();
        Data = ItemData.Parse(p);
        IsRecyclable = p.Read<bool>();
        IsTradeable = p.Read<bool>();
        IsGroupable = p.Read<bool>();
        IsSellable = p.Read<bool>();
        SecondsToExpiration = p.Read<int>();
        HasRentPeriodStarted = p.Read<bool>();
        RoomId = p.Read<Id>();

        if (p.Client == ClientType.Unity)
        {
            // - Seems to be consistent
            _Short1 = p.Read<short>(); // ?
            SlotId = p.Read<string>(); // string "r" / "s"
            _Int3 = p.Read<int>(); // int 1187551480
        }

        if (Type == ItemType.Floor)
        {
            if (p.Client == ClientType.Flash)
            {
                SlotId = p.Read<string>();
                Extra = p.Read<int>();
            }
            else
            {
                // 10 bytes ?
                _String3 = p.Read<string>();
                Extra = p.Read<int>();
                _Int5 = p.Read<int>();
            }
        }
        else
        {
            _String3 = string.Empty;
        }
    }

    private void ParseOrigins(in PacketReader p)
    {
        Kind = -1;
        ItemId = p.Read<int>();
        SlotId = p.Read<int>().ToString();
        string strItemType = p.Read<string>();
        Type = strItemType switch
        {
            "S" => ItemType.Floor,
            "I" => ItemType.Wall,
            _ => throw new Exception($"Invalid item type: {strItemType}"),
        };
        Id = p.Read<int>();
        Identifier = p.Read<string>();
        if (Type == ItemType.Floor)
        {
            p.Read<int>(); // dimX
            p.Read<int>(); // dimY
            // colors
            Data = new LegacyData { Value = p.Read<string>() };
        }
        else if (Type == ItemType.Wall)
        {
            // props
            Data = new LegacyData { Value = p.Read<string>() };
        }
    }

    public void Compose(in PacketWriter p)
    {
        // TODO origins composer
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.Write(ItemId);

        if (p.Client == ClientType.Flash)
        {
            p.Write(Type.ToShortString().ToUpper());
        }
        else
        {
            p.Write(Type.GetValue());
        }

        p.Write(Id);
        p.Write(Kind);
        p.Write((int)Category);
        p.Write(Data);
        p.Write(IsRecyclable);
        p.Write(IsTradeable);
        p.Write(IsGroupable);
        p.Write(IsSellable);
        p.Write(SecondsToExpiration);
        p.Write(HasRentPeriodStarted);
        p.Write(RoomId);

        if (p.Client == ClientType.Unity)
        {
            p.Write(_Short1);
            p.Write(SlotId);
            p.Write(_Int3);
        }

        if (Type == ItemType.Floor)
        {
            if (p.Client == ClientType.Flash)
            {
                p.Write(SlotId);
                p.Write(Extra);
            }
            else
            {
                // 10 bytes ?
                p.Write(_String3);
                p.Write(Extra);
                p.Write(_Int5);
            }
        }
    }

    public static InventoryItem Parse(in PacketReader p) => new(in p);

    public override string ToString() => $"{nameof(InventoryItem)}#{ItemId}/{Type}:{Kind}";
}
