using System;

using Newtonsoft.Json;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public abstract class Entity : IEntity, IWritable
    {
        public bool IsRemoved { get; set; }

        public EntityType Type { get; }

        public int Id { get; }
        public int Index { get;  }

        public string Name { get; set; }
        public string Motto { get; set; }
        public string Figure { get; set; }

        public Tile Location { get; set; }
        ITile IEntity.Location => Location;
        [JsonIgnore] public int X => Location.X;
        [JsonIgnore] public int Y => Location.Y;
        [JsonIgnore] public (int X, int Y) XY => Location.XY;
        [JsonIgnore] public double Z => Location.Z;
        public int Direction { get; set; }

        // States
        public int Dance { get; set; }
        public bool IsIdle { get; set; }
        public bool IsTyping { get; set; }
        public int HandItem { get; set; }
        public int Effect { get; set; }

        public EntityUpdate CurrentUpdate { get; private set; }
        IEntityUpdate IEntity.CurrentUpdate => CurrentUpdate;
        public EntityUpdate PreviousUpdate { get; private set; }
        IEntityUpdate IEntity.PreviousUpdate => PreviousUpdate;

        protected Entity(EntityType type, int id, int index)
        {
            Type = type;
            Id = id;
            Index = index;

            Name = "";
            Motto = "";
            Figure = "";
            Location = new Tile();
            Direction = 0;
        }

        public void Update(EntityUpdate update)
        {
            if (update.Index != Index)
                throw new InvalidOperationException("Entity update index does not match the index of this entity.");

            Location = update.Location;
            Direction = update.Direction;

            OnUpdate(update);

            PreviousUpdate = CurrentUpdate;
            CurrentUpdate = update;
        }

        protected virtual void OnUpdate(EntityUpdate update) { }

        public virtual void Write(Packet packet)
        {
            packet.WriteValues(
                Id,
                Name,
                Motto,
                Figure,
                Index,
                Location,
                (int)Direction,
                (int)Type
            );
        }

        public static Entity Parse(Packet packet)
        {
            int id = packet.ReadInt();
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

        public override string ToString() => $"{Name} (id:{Id})";
    }
}
