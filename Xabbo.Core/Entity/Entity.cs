using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public abstract class Entity : IEntity, IPacketData
    {
        public bool IsRemoved { get; set; }
        public bool IsHidden { get; set; }

        public EntityType Type { get; }

        public long Id { get; }
        public int Index { get;  }

        public string Name { get; set; }
        public string Motto { get; set; }
        public string Figure { get; set; }

        public Tile Location { get; set; }
        [JsonIgnore] public int X => Location.X;
        [JsonIgnore] public int Y => Location.Y;
        [JsonIgnore] public (int X, int Y) XY => Location.XY;
        [JsonIgnore] public float Z => Location.Z;
        [JsonIgnore] public (int X, int Y, float Z) XYZ => Location.XYZ;
        public int Direction { get; set; }

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

        protected Entity(EntityType type, long id, int index)
        {
            Type = type;
            Id = id;
            Index = index;

            Name = "";
            Motto = "";
            Figure = "";
            Location = Tile.Zero;
            Direction = 0;
        }

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

        public virtual void Write(IPacket packet)
        {
            packet.WriteValues(
                Id,
                Name,
                Motto,
                Figure,
                Index,
                Location,
                Direction,
                (int)Type
            );
        }

        public static Entity Parse(IReadOnlyPacket packet)
        {
            long id = packet.ReadLong();
            string name = packet.ReadString();
            string motto = packet.ReadString();
            string figure = packet.ReadString();
            int index = packet.ReadInt();
            var tile = Tile.Parse(packet);
            int dir = packet.ReadInt();
            var type = (EntityType)packet.ReadInt();

            Entity entity;

            switch (type)
            {
                case EntityType.User: entity = new RoomUser(id, index, packet); break;
                case EntityType.Pet: entity = new Pet(id, index, packet); break;
                case EntityType.PublicBot: 
                case EntityType.PrivateBot:
                    entity = new Bot(type, id, index, packet);
                    break;
                default: throw new Exception($"Unknown entity type: {type}");
            }

            entity.Name = name;
            entity.Motto = motto;
            entity.Figure = figure;
            entity.Location = tile;
            entity.Direction = dir;

            return entity;
        }

        public static Entity[] ParseAll(IReadOnlyPacket packet)
        {
            int n = packet.ReadInt();
            var entities = new Entity[n];
            for (int i = 0; i < n; i++)
                entities[i] = Parse(packet);
            return entities;
        }

        public static void WriteAll(IPacket packet, IEnumerable<IEntity> entities)
        {
            packet.WriteInt(entities.Count());
            foreach (var entity in entities)
                entity.Write(packet);
        }

        public static void WriteAll(IPacket packet, params IEntity[] entities) => WriteAll(packet, (IEnumerable<IEntity>)entities);

        public override string ToString() => Name;
    }
}
