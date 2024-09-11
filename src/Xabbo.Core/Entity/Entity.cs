using System;
using System.Text.Json.Serialization;

using Xabbo.Messages;

namespace Xabbo.Core;

public abstract class Entity(EntityType type, Id id, int index) : IEntity, IParserComposer<Entity>
{
    public bool IsRemoved { get; set; }
    public bool IsHidden { get; set; }

    public EntityType Type { get; } = type;

    public Id Id { get; } = id;
    public int Index { get; } = index;

    public string Name { get; set; } = "";
    public string Motto { get; set; } = "";
    public string Figure { get; set; } = "";

    public Tile Location { get; set; } = default;
    [JsonIgnore] public int X => Location.X;
    [JsonIgnore] public int Y => Location.Y;
    [JsonIgnore] public Point XY => Location.XY;
    [JsonIgnore] public float Z => Location.Z;
    public int Direction { get; set; } = 0;
    public Area Area => new(Location.XY, 1, 1);

    // States
    public Dances Dance { get; set; }
    public bool IsIdle { get; set; }
    public bool IsTyping { get; set; }
    public int HandItem { get; set; }
    public int Effect { get; set; }

    public EntityStatusUpdate? CurrentUpdate { get; private set; }
    IEntityStatusUpdate? IEntity.CurrentUpdate => CurrentUpdate;
    public EntityStatusUpdate? PreviousUpdate { get; private set; }
    IEntityStatusUpdate? IEntity.PreviousUpdate => PreviousUpdate;

    public void Update(EntityStatusUpdate update)
    {
        if (update.Index != Index)
            throw new InvalidOperationException("Entity update index does not match the index of this entity.");

        Location = update.Location;
        Direction = update.Direction;

        OnUpdate(update);

        PreviousUpdate = CurrentUpdate;
        CurrentUpdate = update;
    }

    protected virtual void OnUpdate(EntityStatusUpdate update) { }

    public virtual void Compose(in PacketWriter p)
    {
        if (p.Client == ClientType.Shockwave)
        {
            RoomUser? user = this as RoomUser;

            p.WriteInt(Index);
            p.WriteString(Name);
            p.WriteString(Figure);
            p.WriteString(user?.Gender.ToShortString() ?? "");
            p.WriteString(Motto);
            p.Compose(Location);
            p.WriteString(user?.FigureExtra ?? "");
            p.WriteString(user?.BadgeCode ?? "");
        }
        else
        {
            p.WriteId(Id);
            p.WriteString(Name);
            p.WriteString(Motto);
            p.WriteString(Figure);
            p.WriteInt(Index);
            p.Compose(Location);
            p.WriteInt(Direction);
            p.WriteInt((int)Type);
        }
    }

    public override string ToString() => Name;

    static Entity IParser<Entity>.Parse(in PacketReader p)
    {
        Id id;
        string name, motto, figure, gender, poolFigure, badgeCode;
        int index, dir;
        Tile tile;
        EntityType type;

        if (p.Client == ClientType.Shockwave)
        {
            id = -1;
            index = p.ReadInt();
            name = p.ReadString();
            figure = p.ReadString();
            gender = p.ReadString();
            motto = p.ReadString();
            tile = p.Parse<Tile>();
            dir = 0;
            poolFigure = p.ReadString();
            badgeCode = p.ReadString();
        }
        else
        {
            id = p.ReadId();
            name = p.ReadString();
            motto = p.ReadString();
            figure = p.ReadString();
            gender = "";
            index = p.ReadInt();
            tile = p.Parse<Tile>();
            dir = p.ReadInt();
            poolFigure = "";
            badgeCode = "";
        }

        type = (EntityType)p.ReadInt();
        Entity entity = type switch
        {
            EntityType.User => new RoomUser(id, index, in p),
            EntityType.Pet => new Pet(id, index, in p),
            EntityType.PublicBot or EntityType.PrivateBot => new Bot(type, id, index, in p),
            _ => throw new Exception($"Unknown entity type: {type}"),
        };

        if (p.Client == ClientType.Shockwave &&
            entity is RoomUser user)
        {
            user.Gender = gender switch
            {
                "M" or "m" => Gender.Male,
                "F" or "f" => Gender.Female,
                _ => Gender.None
            };
            user.FigureExtra = poolFigure;
            user.BadgeCode = badgeCode;
        }

        entity.Name = name;
        entity.Motto = motto;
        entity.Figure = figure;
        entity.Location = tile;
        entity.Direction = dir;

        return entity;
    }
}
