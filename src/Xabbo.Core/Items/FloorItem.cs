﻿using System;
using System.Text.Json.Serialization;

using Xabbo.Messages;

namespace Xabbo.Core;

public class FloorItem : Furni, IFloorItem, IParserComposer<FloorItem>
{
    public override ItemType Type => ItemType.Floor;

    public Tile Location { get; set; }
    public Point? Dimensions { get; set; }
    public Area Area
    {
        get
        {
            if (Dimensions is { } dimensions)
            {
                Area area = new((X, Y), dimensions.X, dimensions.Y);
                if (Direction == 2 || Direction == 6)
                    area = area.Flip();
                return area;
            }
            return Extensions.GetArea(this);
        }
    }
    [JsonIgnore] public int X => Location.X;
    [JsonIgnore] public int Y => Location.Y;
    [JsonIgnore] public Point XY => Location.XY;
    [JsonIgnore] public double Z => Location.Z;
    public int Direction { get; set; }
    public float Height { get; set; }
    public long Extra { get; set; }

    public ItemData Data { get; set; } = new EmptyItemData();
    IItemData IFloorItem.Data => Data;

    public override int State => double.TryParse(Data.Value, out double state) ? (int)state : -1;

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
        Identifier = item.Identifier;
    }

    protected FloorItem(in PacketReader p)
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
        Id = p.ReadId();
        Kind = p.ReadInt();
        int x = p.ReadInt();
        int y = p.ReadInt();
        Direction = p.ReadInt();
        float z = p.ReadFloat();
        Location = new Tile(x, y, z);
        Height = p.ReadFloat();
        Extra = p.ReadId();

        Data = p.Parse<ItemData>();

        SecondsToExpiration = p.ReadInt();
        Usage = (FurniUsage)p.ReadInt();
        OwnerId = p.ReadId();

        if (Kind < 0)
        {
            Identifier = p.ReadString();
        }
        else
        {
            Identifier = string.Empty;
        }
    }

    private void ParseOrigins(in PacketReader p)
    {
        Id = (Id)p.ReadString();
        Identifier = p.ReadString();
        int x = p.ReadInt();
        int y = p.ReadInt();
        Dimensions = new Point(p.ReadInt(), p.ReadInt());
        Direction = p.ReadInt();
        float z = p.ReadFloat();
        Location = new Tile(x, y, z);
        p.ReadString(); // colors
        p.ReadString(); // runtimeData
        Extra = p.ReadInt();
        Data = new LegacyData { Value = p.ReadString() };

        Kind = -1;
        OwnerId = -1;
    }

    protected override void Compose(in PacketWriter p)
    {
        if (p.Client == ClientType.Shockwave)
            throw new NotImplementedException("FloorItem.Compose is not implemented for Shockwave");

        p.WriteId(Id);
        p.WriteInt(Kind);
        p.WriteInt(Location.X);
        p.WriteInt(Location.Y);
        p.WriteInt(Direction);
        p.WriteFloat(Location.Z);
        p.WriteFloat(Height);
        p.WriteId(Extra);
        p.Compose(Data);
        p.WriteInt(SecondsToExpiration);
        p.WriteInt((int)Usage);
        p.WriteId(OwnerId);

        if (Kind < 0) p.WriteString(Identifier);
    }

    public override string ToString() => $"{nameof(FloorItem)}#{Id}/{Kind}";

    static FloorItem IParser<FloorItem>.Parse(in PacketReader p) => new(in p);
}
