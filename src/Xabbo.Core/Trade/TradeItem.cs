using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="ITradeItem"/>
public class TradeItem : ITradeItem, IParserComposer<TradeItem>
{
    public Id ItemId { get; set; }
    public ItemType Type { get; set; }
    public bool IsFloorItem => Type == ItemType.Floor;
    public bool IsWallItem => Type == ItemType.Wall;
    public Id Id { get; set; }
    public int Kind { get; set; }
    public string Identifier { get; set; } = "";
    public FurniCategory Category { get; set; }
    public bool IsGroupable { get; set; }
    public ItemData Data { get; set; } = new LegacyData();
    IItemData IInventoryItem.Data => Data;
    public int CreationDay { get; set; }
    public int CreationMonth { get; set; }
    public int CreationYear { get; set; }
    public long Extra { get; set; }
    public Point? Size { get; set; }
    public string SlotId { get; set; } = "";

    bool IInventoryItem.IsRecyclable => true;
    bool IInventoryItem.IsTradeable => true;
    bool IInventoryItem.IsSellable => true;
    string IInventoryItem.SlotId => "";
    int IInventoryItem.SecondsToExpiration => -1;
    bool IInventoryItem.HasRentPeriodStarted => false;
    Id IInventoryItem.RoomId => -1;

    public TradeItem() { }

    protected TradeItem(in PacketReader p)
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
        Type = H.ToItemType(p.ReadString());
        Id = p.ReadId();
        Kind = p.ReadInt();
        Category = (FurniCategory)p.ReadInt();
        IsGroupable = p.ReadBool();
        Data = p.Parse<ItemData>();
        CreationDay = p.ReadInt();
        CreationMonth = p.ReadInt();
        CreationYear = p.ReadInt();

        if (Type == ItemType.Floor)
            Extra = p.ReadId();
        else
            Extra = -1;
    }

    private void ParseOrigins(in PacketReader p)
    {
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
            Size = (p.ReadInt(), p.ReadInt());

        // Colors (FloorItem), Props (WallItem)
        Data = new LegacyData { Value = p.ReadString() };
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
            ComposeOrigins(in p);
        else
            ComposeModern(in p);
    }

    void ComposeOrigins(in PacketWriter p)
    {
        p.WriteId(ItemId);
        p.WriteInt(int.TryParse(SlotId, out int slotId) ? slotId : 0);
        p.WriteString(Type switch
        {
            ItemType.Floor => "S",
            ItemType.Wall => "I",
            _ => throw new Exception($"Invalid item type: {Type}.")
        });
        p.WriteId(Id);
        p.WriteString(Identifier);
        if (Type == ItemType.Floor)
        {
            p.WriteInt(Size?.X ?? 1);
            p.WriteInt(Size?.Y ?? 1);
        }
        p.WriteString(Data.Value);
    }

    void ComposeModern(in PacketWriter p)
    {

        p.WriteId(ItemId);
        p.WriteString(Type.GetClientIdentifier());
        p.WriteId(Id);
        p.WriteInt(Kind);
        p.WriteInt((int)Category);
        p.WriteBool(IsGroupable);
        p.Compose(Data);
        p.WriteInt(CreationDay);
        p.WriteInt(CreationMonth);
        p.WriteInt(CreationYear);

        if (Type == ItemType.Floor)
            p.WriteId(Extra);
    }

    public override string ToString() => $"{nameof(TradeItem)}#{ItemId}/{Type}:{Kind}";

    static TradeItem IParser<TradeItem>.Parse(in PacketReader p) => new(in p);
}
