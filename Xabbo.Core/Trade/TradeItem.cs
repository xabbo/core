using System;
using Xabbo.Messages;

namespace Xabbo.Core;

public class TradeItem : ITradeItem
{
    public long ItemId { get; set; }
    public ItemType Type { get; set; }
    public bool IsFloorItem => Type == ItemType.Floor;
    public bool IsWallItem => Type == ItemType.Wall;
    public long Id { get; set; }
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

    protected TradeItem(IReadOnlyPacket packet)
    {
        ItemId = packet.ReadLegacyLong();
        Type = H.ToItemType(packet.ReadString());
        Id = packet.ReadLegacyLong();
        Kind = packet.ReadInt();
        Category = (FurniCategory)packet.ReadInt();
        IsGroupable = packet.ReadBool();
        Data = ItemData.Parse(packet);
        CreationDay = packet.ReadInt();
        CreationMonth = packet.ReadInt();
        CreationYear = packet.ReadInt();

        if (Type == ItemType.Floor)
        {
            Extra = packet.ReadLegacyLong();
        }
        else
        {
            Extra = -1;
        }
    }

    public void Compose(IPacket packet)
    {
        packet
            .WriteLegacyLong(ItemId)
            .WriteString(Type.ToShortString())
            .WriteLegacyLong(Id)
            .WriteInt(Kind)
            .WriteInt((int)Category)
            .WriteBool(IsGroupable)
            .Write(Data)
            .WriteInt(CreationDay)
            .WriteInt(CreationMonth)
            .WriteInt(CreationYear);

        if (Type == ItemType.Floor)
        {
            packet.WriteLegacyLong(Extra);
        }
    }

    public static TradeItem Parse(IReadOnlyPacket packet)
    {
        return new TradeItem(packet);
    }

    public override string ToString() => $"{ItemId}:{Type.ToShortString()}{Kind}";
}
