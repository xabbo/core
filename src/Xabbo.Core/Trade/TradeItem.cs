using Xabbo.Messages;

namespace Xabbo.Core;

public class TradeItem : ITradeItem, IComposer, IParser<TradeItem>
{
    public Id ItemId { get; set; }
    public ItemType Type { get; set; }
    public bool IsFloorItem => Type == ItemType.Floor;
    public bool IsWallItem => Type == ItemType.Wall;
    public Id Id { get; set; }
    public int Kind { get; set; }
    public FurniCategory Category { get; set; }
    public bool IsGroupable { get; set; }
    public ItemData Data { get; set; } = new LegacyData();
    IItemData IInventoryItem.Data => Data;
    public int CreationDay { get; set; }
    public int CreationMonth { get; set; }
    public int CreationYear { get; set; }
    public long Extra { get; set; }

    bool IInventoryItem.IsRecyclable => true;
    bool IInventoryItem.IsTradeable => true;
    bool IInventoryItem.IsSellable => true;
    string IInventoryItem.SlotId => string.Empty;
    int IInventoryItem.SecondsToExpiration => -1;
    bool IInventoryItem.HasRentPeriodStarted => false;
    long IInventoryItem.RoomId => -1;

    public TradeItem() { }

    protected TradeItem(in PacketReader p)
    {
        ItemId = p.Read<Id>();
        Type = H.ToItemType(p.Read<string>());
        Id = p.Read<Id>();
        Kind = p.Read<int>();
        Category = (FurniCategory)p.Read<int>();
        IsGroupable = p.Read<bool>();
        Data = ItemData.Parse(p);
        CreationDay = p.Read<int>();
        CreationMonth = p.Read<int>();
        CreationYear = p.Read<int>();

        if (Type == ItemType.Floor)
            Extra = p.Read<Id>();
        else
            Extra = -1;
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(ItemId);
        p.Write(Type.ToShortString());
        p.Write(Id);
        p.Write(Kind);
        p.Write((int)Category);
        p.Write(IsGroupable);
        p.Write(Data);
        p.Write(CreationDay);
        p.Write(CreationMonth);
        p.Write(CreationYear);

        if (Type == ItemType.Floor)
            p.Write<Id>(Extra);
    }

    public static TradeItem Parse(in PacketReader packet) => new(in packet);

    public override string ToString() => $"{nameof(TradeItem)}#{ItemId}/{Type}:{Kind}";
}
