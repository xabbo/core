using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class InventoryItem : IInventoryItem, IComposer, IParser<InventoryItem>
{
    public Id ItemId { get; set; }
    public ItemType Type { get; set; }
    public Id Id { get; set; }
    public int Kind { get; set; }
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

        /*if (clientType == ClientType.Flash)
        {
            ItemId = packet.Read<int>();
            Type = H.ToItemType(packet.Read<string>());
            Id = packet.Read<int>();
            Kind = packet.Read<int>();
            Category = (FurniCategory)packet.Read<int>();
            Data = StuffData.Parse(packet, clientType);
            _Bool1 = packet.Read<bool>();
            IsTradeable = packet.Read<bool>();
            IsGroupable = packet.Read<bool>();
            IsSellable = packet.Read<bool>();
            SecondsToExpiration = packet.Read<int>();
            HasRentPeriodStarted = packet.Read<bool>();
            RoomId = packet.Read<int>();

            if (Type == ItemType.Floor)
            {
                _String2 = packet.Read<string>();
                Extra = packet.Read<int>();
            }
        }
        else
        {
            ItemId = packet.Read<long>();
            Type = H.ToItemType(packet.Read<short>());
            Id = packet.Read<long>();
            Kind = packet.Read<int>();
            Category = (FurniCategory)packet.Read<int>();
            Data = StuffData.Parse(packet, clientType);
            _Bool1 = packet.Read<bool>();
            IsTradeable = packet.Read<bool>();
            IsGroupable = packet.Read<bool>();
            IsSellable = packet.Read<bool>();
            SecondsToExpiration = packet.Read<int>();
            HasRentPeriodStarted = packet.Read<bool>();
            RoomId = packet.Read<long>();

            // - Seems to be consistent
            _String1 = packet.Read<string>(); // string ""
            _String2 = packet.Read<string>(); // string "r" / "s"
            _Int3 = packet.Read<int>(); // int 1187551480

            if (Type == ItemType.Floor)
            {
                // 10 bytes ?
                _String3 = packet.Read<string>();
                Extra = packet.Read<int>();
                _Int5 = packet.Read<int>();
            }
            else
            {
                _String3 = string.Empty;
            }
        }*/
    }

    public void Compose(in PacketWriter p)
    {
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
