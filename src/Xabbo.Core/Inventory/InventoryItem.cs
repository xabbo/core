using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IInventoryItem"/>
public sealed class InventoryItem : IInventoryItem, IParserComposer<InventoryItem>
{
    public Id ItemId { get; set; }
    public ItemType Type { get; set; }
    public Id Id { get; set; }
    public int Kind { get; set; }
    public string? Identifier { get; set; }
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
    public string SlotId { get; set; } = "";
    public long Extra { get; set; }
    public bool IsFloorItem => Type is ItemType.Floor;
    public bool IsWallItem => Type is ItemType.Wall;

    public Point? Size { get; set; }

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
        Data = ItemData.Clone(item.Data);
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

        if (p.Client is ClientType.Flash)
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

        if (p.Client is ClientType.Unity)
        {
            // TODO Unity
            // p.ReadShort(); // ?
            // p.ReadString(); // string "r" / "s"
            // p.ReadInt();
        }

        if (Type is ItemType.Floor)
        {
            if (p.Client is ClientType.Flash)
            {
                SlotId = p.ReadString();
                Extra = p.ReadInt();
            }
            else
            {
                // TODO Unity
            }
        }
    }

    private void ParseOrigins(in PacketReader p)
    {
        ItemId = p.ReadId();
        SlotId = p.ReadInt().ToString();

        string strItemType = p.ReadString();
        Type = strItemType switch
        {
            "S" => ItemType.Floor,
            "I" => ItemType.Wall,
            _ => throw new Exception($"Invalid item type: {strItemType}"),
        };

        Id = p.ReadId();
        Identifier = p.ReadString();
        if (Type is ItemType.Floor)
            Size = (p.ReadInt(), p.ReadInt());

        Data = new LegacyData { Value = p.ReadString() };
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            ComposeOrigins(in p);
        }
        else
        {
            ComposeModern(in p);
        }
    }

    private void ComposeOrigins(in PacketWriter p)
    {
        p.WriteId(ItemId);
        if (!int.TryParse(SlotId, out int slot))
            slot = 0;
        p.WriteInt(slot);
        p.WriteString(Type.GetClientIdentifier().ToUpper());
        p.WriteId(Id);
        p.WriteString(Identifier ?? "");

        if (Type is ItemType.Floor)
        {
            p.WriteInt(Size?.X ?? 1);
            p.WriteInt(Size?.Y ?? 1);
            p.WriteString(Data.Value);
        }
        else if (Type is ItemType.Wall)
        {
            p.WriteString(Data.Value);
        }
    }

    private void ComposeModern(in PacketWriter p)
    {
        p.WriteId(ItemId);

        if (p.Client is ClientType.Flash)
        {
            p.WriteString(Type.GetClientIdentifier().ToUpper());
        }
        else
        {
            p.WriteShort(Type.GetClientValue());
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

        // if (p.Client is ClientType.Unity)
        // {
        //     p.WriteShort(_Short1);
        //     p.WriteString(SlotId);
        //     p.WriteInt(_Int3);
        // }

        if (Type is ItemType.Floor)
        {
            if (p.Client is ClientType.Flash)
            {
                p.WriteString(SlotId);
                p.WriteInt((int)Extra);
            }
            else
            {
                // Unity
            }
        }

    }

    static InventoryItem IParser<InventoryItem>.Parse(in PacketReader p) => new(in p);

    public override string ToString() => $"{nameof(InventoryItem)}#{ItemId}";
}
