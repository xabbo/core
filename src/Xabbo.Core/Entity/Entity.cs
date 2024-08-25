using System;
using System.Collections.Generic;
using System.Linq;
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
        p.Write(Id);
        p.Write(Name);
        p.Write(Motto);
        p.Write(Figure);
        p.Write(Index);
        p.Write(Location);
        p.Write(Direction);
        p.Write((int)Type);
    }

    public override string ToString() => Name;

    public static Entity Parse(in PacketReader p)
    {
        Id id = p.Read<Id>();
        string name = p.Read<string>();
        string motto = p.Read<string>();
        string figure = p.Read<string>();
        int index = p.Read<int>();
        Tile tile = p.Parse<Tile>();
        int dir = p.Read<int>();
        EntityType type = (EntityType)p.Read<int>();

        Entity entity = type switch
        {
            EntityType.User => new RoomUser(id, index, in p),
            EntityType.Pet => new Pet(id, index, in p),
            EntityType.PublicBot or EntityType.PrivateBot => new Bot(type, id, index, in p),
            _ => throw new Exception($"Unknown entity type: {type}"),
        };

        entity.Name = name;
        entity.Motto = motto;
        entity.Figure = figure;
        entity.Location = tile;
        entity.Direction = dir;

        return entity;
    }
}
