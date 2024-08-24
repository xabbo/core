using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using Xabbo.Messages;

namespace Xabbo.Core;

public class FloorItem : Furni, IFloorItem, IComposer, IParser<FloorItem>, IManyParser<FloorItem>
{
    public override ItemType Type => ItemType.Floor;

    public Tile Location { get; set; }
    public Area Area => Extensions.XabboCoreExtensions.GetArea(this);
    [JsonIgnore] public int X => Location.X;
    [JsonIgnore] public int Y => Location.Y;
    [JsonIgnore] public Point XY => Location.XY;
    [JsonIgnore] public double Z => Location.Z;
    public int Direction { get; set; }
    public float Height { get; set; }
    public long Extra { get; set; }

    public ItemData Data { get; set; }
    IItemData IFloorItem.Data => Data;

    public override int State => double.TryParse(Data.Value, out double state) ? (int)state : -1;

    public string StaticClass { get; set; }

    public FloorItem()
    {
        Location = default;
        Data = new LegacyData();
        SecondsToExpiration = -1;
        Usage = FurniUsage.None;
        StaticClass = string.Empty;
    }

    public FloorItem(IFloorItem item)
    {
        Id = item.Id;
        Kind = item.Kind;
        Location = item.Location;
        Direction = item.Direction;
        Height = item.Height;
        Extra = item.Extra;
        Data = ItemData.Clone(item.Data);
        SecondsToExpiration = item.SecondsToExpiration;
        Usage = item.Usage;
        OwnerId = item.OwnerId;
        OwnerName = item.OwnerName;
        StaticClass = item.StaticClass;
    }

    protected FloorItem(in PacketReader p, bool readName)
    {
        Id = p.Read<Id>();
        Kind = p.Read<int>();
        int x = p.Read<int>();
        int y = p.Read<int>();
        Direction = p.Read<int>();
        float z = p.Read<float>();
        Location = new Tile(x, y, z);
        Height = p.Read<float>();
        Extra = p.Read<Id>();
        // - consumable state e.g. cabbage 0: full, 1: partly eaten, 2: mostly eaten
        // - linked teleport id

        Data = p.Parse<ItemData>();

        SecondsToExpiration = p.Read<int>();
        Usage = (FurniUsage)p.Read<int>();
        OwnerId = p.Read<Id>();

        if (Kind < 0)
        {
            StaticClass = p.Read<string>();
        }
        else
        {
            StaticClass = string.Empty;
        }

        if (readName && p.Available >= 2)
            OwnerName = p.Read<string>();
    }

    public override void Compose(in PacketWriter p) => Compose(in p, true);
    public override void Compose(in PacketWriter p, bool writeOwnerName)
    {
        p.Write(Id);
        p.Write(Kind);
        p.Write(Location.X);
        p.Write(Location.Y);
        p.Write(Direction);
        p.Write(Location.Z);
        p.Write(Height);
        p.Write(Extra);
        p.Write(Data);
        p.Write(SecondsToExpiration);
        p.Write((int)Usage);
        p.Write(OwnerId);

        if (Kind < 0) p.Write(StaticClass);
        if (writeOwnerName) p.Write(OwnerName);
    }

    public override string ToString() => $"{nameof(FloorItem)}#{Id}/{Kind}";

    public static FloorItem Parse(in PacketReader p) => Parse(in p, true);
    public static FloorItem Parse(in PacketReader p, bool readName) => new(in p, readName);
    public static FloorItem ParseUpdate(in PacketReader p) => new(in p, false);

    public static IEnumerable<FloorItem> ParseAll(in PacketReader p)
    {
        var ownerDictionary = new Dictionary<long, string>();

        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
            ownerDictionary.Add(p.Read<Id>(), p.Read<string>());

        n = p.Read<Length>();
        var items = new FloorItem[n];
        for (int i = 0; i < n; i++)
        {
            var item = items[i] = Parse(p, false);
            if (ownerDictionary.TryGetValue(item.OwnerId, out string? ownerName))
                item.OwnerName = ownerName;
        }

        return items;
    }

    public static void ComposeAll(in PacketWriter p, IEnumerable<IFloorItem> items)
    {
        var ownerIds = new HashSet<long>();
        var ownerDictionary = items
            .Where(x => ownerIds.Add(x.OwnerId))
            .ToDictionary(
                key => key.OwnerId,
                val => val.OwnerName
            );

        p.Write<Length>(ownerDictionary.Count);
        foreach (var pair in ownerDictionary)
        {
            p.Write(pair.Key);
            p.Write(pair.Value);
        }

        p.Write<Length>(items.Count());
        foreach (IFloorItem item in items)
            item.Compose(p, false);
    }

    public static void ComposeAll(in PacketWriter p, params FloorItem[] items)
        => ComposeAll(in p, (IEnumerable<IFloorItem>)items);
}
