using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using Xabbo.Messages;

namespace Xabbo.Core;

public class WallItem : Furni, IWallItem, IComposer, IParser<WallItem>, IManyParser<WallItem>
{
    public override ItemType Type => ItemType.Wall;

    public string Data { get; set; }

    public override int State => int.TryParse(Data, out int state) ? state : -1;

    public WallLocation Location { get; set; }

    [JsonIgnore]
    public int WX => Location.WX;

    [JsonIgnore]
    public int WY => Location.WY;

    [JsonIgnore]
    public int LX => Location.LX;

    [JsonIgnore]
    public int LY => Location.LY;

    [JsonIgnore]
    public WallOrientation Orientation => Location.Orientation;

    public WallItem()
    {
        OwnerId = -1;
        OwnerName = "(unknown)";

        Data = string.Empty;
        SecondsToExpiration = -1;
        Usage = FurniUsage.None;
        Location = WallLocation.Zero;
    }

    /// <summary>
    /// Creates a copy of the specified wall item.
    /// </summary>
    public WallItem(IWallItem item)
    {
        Id = item.Id;
        Kind = item.Kind;
        Location = item.Location;
        Data = item.Data;
        SecondsToExpiration = item.SecondsToExpiration;
        Usage = item.Usage;

        OwnerId = item.OwnerId;
        OwnerName = item.OwnerName;

        IsHidden = item.IsHidden;
    }

    protected WallItem(in PacketReader p, bool readName)
        : this()
    {
        Id = p.Client switch
        {
            ClientType.Flash => long.Parse(p.Read<string>()),
            ClientType.Unity => p.Read<long>(),
            _ => throw new InvalidOperationException("Unknown protocol.")
        };
        Kind = p.Read<int>();
        Location = WallLocation.Parse(p.Read<string>());
        Data = p.Read<string>();
        SecondsToExpiration = p.Read<int>();
        Usage = (FurniUsage)p.Read<int>();
        OwnerId = p.Read<Id>();

        if (readName && p.Available >= 2)
            OwnerName = p.Read<string>();
    }

    public override void Compose(in PacketWriter p) => Compose(in p, true);

    public override void Compose(in PacketWriter p, bool writeOwnerName = true)
    {
        if (p.Client == ClientType.Flash) p.Write(Id.ToString());
        else if (p.Client == ClientType.Unity) p.Write(Id);
        else throw new InvalidOperationException("Unknown protocol");

        p.Write(Kind);
        p.Write(Location.ToString());
        p.Write(Data);
        p.Write(SecondsToExpiration);
        p.Write((int)Usage);
        p.Write(OwnerId);

        if (writeOwnerName)
        {
            p.Write(OwnerName);
        }
    }

    public override string ToString() => $"{nameof(WallItem)}#{Id}/{Kind}";

    public static WallItem Parse(in PacketReader p) => Parse(in p, true);
    public static WallItem Parse(in PacketReader packet, bool readName) => new(in packet, readName);

    public static IEnumerable<WallItem> ParseAll(in PacketReader p)
    {
        var ownerDictionary = new Dictionary<long, string>();

        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
            ownerDictionary.Add(p.Read<Id>(), p.Read<string>());

        n = p.Read<Length>();
        WallItem[] wallItems = new WallItem[n];
        for (int i = 0; i < n; i++)
        {
            var item = wallItems[i] = Parse(p, false);
            if (ownerDictionary.TryGetValue(item.OwnerId, out string? ownerName))
                item.OwnerName = ownerName;
        }

        return wallItems;
    }

    public static void ComposeAll(in PacketWriter p, IEnumerable<IWallItem> items)
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
        foreach (var item in items)
            item.Compose(in p, false);
    }

    public static void ComposeAll(in PacketWriter p, params WallItem[] items) => ComposeAll(p, (IEnumerable<IWallItem>)items);
}
