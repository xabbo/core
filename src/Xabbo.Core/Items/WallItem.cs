using System;

using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IWallItem"/>
public class WallItem : Furni, IWallItem, IParserComposer<WallItem>
{
    public override ItemType Type => ItemType.Wall;

    public string Data { get; set; } = "";

    public override int State => int.TryParse(Data, out int state) ? state : -1;

    public WallLocation Location { get; set; } = WallLocation.Zero;

    public int WX => Location.Wall.X;
    public int WY => Location.Wall.Y;
    public int LX => Location.Offset.X;
    public int LY => Location.Offset.Y;
    public WallOrientation Orientation => Location.Orientation;

    /// <summary>
    /// Constructs a new empty wall item.
    /// </summary>
    public WallItem() { }

    /// <summary>
    /// Constructs a new copy of the specified wall item.
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

    protected WallItem(in PacketReader p)
        : this()
    {
        switch (p.Client)
        {
            case ClientType.Unity or ClientType.Flash:
                ParseModern(in p);
                break;
            default:
                throw new UnsupportedClientException(p.Client);
        }
    }

    private void ParseModern(in PacketReader p)
    {
        Id = p.Client switch
        {
            ClientType.Flash => (Id)p.ReadString(),
            ClientType.Unity => p.ReadLong(),
            _ => throw new UnsupportedClientException(p.Client)
        };
        Kind = p.ReadInt();
        Location = WallLocation.ParseString(p.ReadString());
        Data = p.ReadString();
        SecondsToExpiration = p.ReadInt();
        Usage = (FurniUsage)p.ReadInt();
        OwnerId = p.ReadId();
    }

    protected override void Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.WriteContent(ToOriginsString());
            return;
        }

        if (p.Client is ClientType.Flash) p.WriteString(Id.ToString());
        else if (p.Client is ClientType.Unity) p.WriteId(Id);
        else throw new UnsupportedClientException(p.Client);

        p.WriteInt(Kind);
        p.WriteString(Location.ToString());
        p.WriteString(Data);
        p.WriteInt(SecondsToExpiration);
        p.WriteInt((int)Usage);
        p.WriteId(OwnerId);
    }

    public override string ToString() => $"{nameof(WallItem)}#{Id}/{Kind}";

    public string ToOriginsString()
    {
        string s = string.Join('\t', Id, Identifier, OwnerName, Location);
        if (!string.IsNullOrWhiteSpace(Data))
            s += "\t" + Data;
        return s;
    }

    static WallItem IParser<WallItem>.Parse(in PacketReader p)
    {
        if (p.Client is ClientType.Shockwave)
            return ParseOriginsString(p.ReadContent());
        else
            return new(in p);
    }

    public static WallItem ParseOriginsString(string value)
    {
        string[] fields = value.Split('\t');
        if (fields.Length is < 4 or > 5)
            throw new Exception($"Invalid number of fields when parsing WallItem: {string.Join(", ", fields)}");

        if (!int.TryParse(fields[0], out int id))
            throw new Exception($"Invalid ID when parsing WallItem: {fields[0]}.");

        WallItem wallItem = new()
        {
            Id = id,
            Identifier = fields[1],
            OwnerId = -1,
            OwnerName = fields[2],
            Location = WallLocation.ParseString(fields[3])
        };
        if (fields.Length >= 5)
            wallItem.Data = fields[4];

        return wallItem;
    }
}
