using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using Xabbo.Messages;

namespace Xabbo.Core;

public class WallItem : Furni, IWallItem, IComposer, IParser<WallItem>, IManyParser<WallItem>
{
    public override ItemType Type => ItemType.Wall;

    public string Data { get; set; } = "";

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
        switch (p.Client)
        {
            case ClientType.Unity or ClientType.Flash:
                ParseModern(in p, readName);
                break;
            case ClientType.Shockwave:
                ParseOrigins(in p);
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }

    private void ParseModern(in PacketReader p, bool readName)
    {
        Id = p.Client switch
        {
            ClientType.Flash => (Id)p.Read<string>(),
            ClientType.Unity => p.Read<long>(),
            _ => throw new UnsupportedClientException(p.Client)
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

    private void ParseOrigins(in PacketReader p) => ParseOriginsString(p.Read<string>().Trim());

    private void ParseOriginsString(string value)
    {
        string[] fields = value.Split('\t');
        if (fields.Length < 4)
            throw new Exception($"Invalid number of fields when parsing WallItem: {string.Join(", ", fields)}");

        if (!int.TryParse(fields[0], out int id))
            throw new Exception($"Invalid ID when parsing WallItem: {fields[0]}.");

        Id = id;
        Identifier = fields[1];
        OwnerId = -1;
        OwnerName = fields[2];
        Location = WallLocation.Parse(fields[3]);
        if (fields.Length >= 5)
            Data = fields[4];
    }

    public override void Compose(in PacketWriter p) => Compose(in p, true);

    public override void Compose(in PacketWriter p, bool writeOwnerName = true)
    {
        if (p.Client == ClientType.Shockwave)
            throw new NotImplementedException("WallItem.ComposeAll is not implemented for Shockwave");

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

    public static void ComposeAll(in PacketWriter p, IEnumerable<IWallItem> items)
    {
        if (p.Client == ClientType.Shockwave)
            throw new NotImplementedException("WallItem.ComposeAll is not implemented for Shockwave");

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

    public static WallItem Parse(in PacketReader p) => Parse(in p, true);
    public static WallItem Parse(in PacketReader p, bool readName) => new(in p, readName);
    public static IEnumerable<WallItem> ParseAll(in PacketReader p)
    {
        if (p.Client == ClientType.Shockwave)
        {
            var items = new List<WallItem>();
            while (p.Available > 0)
            {
                items.Add(p.Parse<WallItem>());
            }
            return items;
        }

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
}
