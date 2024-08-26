using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

using Xabbo.Messages;

namespace Xabbo.Core;

public abstract class Entity(EntityType type, Id id, int index) : IEntity, IComposer, IParser<Entity>
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
    public int Dance { get; set; }
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

            p.Write(Index);
            p.Write(Name);
            p.Write(Figure);
            p.Write(user?.Gender.ToShortString() ?? "");
            p.Write(Motto);
            p.Write(Location);
            p.Write(user?.FigureExtra ?? "");
            p.Write(user?.BadgeCode ?? "");
        }
        else
        {
            p.Write(Id);
            p.Write(Name);
            p.Write(Motto);
            p.Write(Figure);
            p.Write(Index);
            p.Write(Location);
            p.Write(Direction);
            p.Write((int)Type);
        }
    }

    public override string ToString() => Name;

    public static Entity Parse(in PacketReader p)
    {
        Id id;
        string name, motto, figure, gender, poolFigure, badgeCode;
        int index, dir;
        Tile tile;
        EntityType type;

        if (p.Client == ClientType.Shockwave)
        {
            id = -1;
            index = p.Read<int>();
            name = p.Read<string>();
            figure = p.Read<string>();
            gender = p.Read<string>();
            motto = p.Read<string>();
            tile = p.Parse<Tile>();
            dir = 0;
            poolFigure = p.Read<string>();
            badgeCode = p.Read<string>();
        }
        else
        {
            id = p.Read<Id>();
            name = p.Read<string>();
            motto = p.Read<string>();
            figure = p.Read<string>();
            gender = "";
            index = p.Read<int>();
            tile = p.Parse<Tile>();
            dir = p.Read<int>();
            poolFigure = "";
            badgeCode = "";
        }

        type = (EntityType)p.Read<int>();
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
