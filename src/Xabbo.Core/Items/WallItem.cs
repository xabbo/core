using System;

using Xabbo.Messages;

namespace Xabbo.Core;

public class WallItem : Furni, IWallItem, IParserComposer<WallItem>
{
    public override ItemType Type => ItemType.Wall;

    public string Data { get; set; } = "";

    public override int State => int.TryParse(Data, out int state) ? state : -1;

    public WallLocation Location { get; set; }

    public int WX => Location.Wall.X;
    public int WY => Location.Wall.Y;
    public int LX => Location.Offset.X;
    public int LY => Location.Offset.Y;
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

    protected WallItem(in PacketReader p)
        : this()
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

    private void ParseOrigins(in PacketReader p) => ParseOriginsString(p.ReadString().Trim());

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
        Location = WallLocation.ParseString(fields[3]);
        if (fields.Length >= 5)
            Data = fields[4];
    }

    protected override void Compose(in PacketWriter p)
    {
        if (p.Client == ClientType.Shockwave)
            throw new UnsupportedClientException(p.Client);

        if (p.Client == ClientType.Flash) p.WriteString(Id.ToString());
        else if (p.Client == ClientType.Unity) p.WriteId(Id);
        else throw new InvalidOperationException("Unknown protocol");

        p.WriteInt(Kind);
        p.WriteString(Location.ToString());
        p.WriteString(Data);
        p.WriteInt(SecondsToExpiration);
        p.WriteInt((int)Usage);
        p.WriteId(OwnerId);
    }

    public override string ToString() => $"{nameof(WallItem)}#{Id}/{Kind}";

    static WallItem IParser<WallItem>.Parse(in PacketReader p) => new(in p);
}
