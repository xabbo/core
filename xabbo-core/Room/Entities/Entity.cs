using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public abstract class Entity : IWritable
    {
        public bool IsRemoved { get; set; }

        public int Id { get; }
        public int Index { get;  }
        public EntityType Type { get; }

        public string Name { get; set; }
        public string Motto { get; set; }
        public string Figure { get; set; }

        public Tile Tile { get; set; }
        public Direction Direction { get; set; }

        // States
        public int Dance { get; set; }
        public bool IsIdle { get; set; }
        public bool IsTyping { get; set; }
        public int HandItem { get; set; }
        public int Effect { get; set; }

        public EntityUpdate CurrentUpdate { get; private set; }
        public EntityUpdate PreviousUpdate { get; private set; }

        protected Entity(EntityType type, int id, int index)
        {
            Type = type;
            Id = id;
            Index = index;
        }

        public void Update(EntityUpdate update)
        {
            if (update.Index != Index)
                throw new InvalidOperationException("Entity update index does not match the index of this entity.");

            Tile = update.Tile;
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
                Tile,
                (int)Direction,
                (int)Type
            );
        }

        public static Entity Parse(Packet packet)
        {
            int id = packet.ReadInteger();
            string name = packet.ReadString();
            string motto = packet.ReadString();
            string figure = packet.ReadString();
            int index = packet.ReadInteger();
            var tile = Tile.Parse(packet);
            var dir = (Direction)packet.ReadInteger();
            var type = (EntityType)packet.ReadInteger();

            Entity entity;

            switch (type)
            {
                case EntityType.User: entity = new RoomUser(id, index, packet); break;
                case EntityType.Pet: entity = new Pet(id, index, packet); break;
                case EntityType.PublicBot: entity = new PublicBot(id, index); break;
                case EntityType.PrivateBot: entity = new PrivateBot(id, index, packet); break;
                default: throw new Exception($"Unknown entity type: {type}");
            }

            entity.Name = name;
            entity.Motto = motto;
            entity.Figure = figure;
            entity.Tile = tile;
            entity.Direction = dir;

            return entity;
        }

        public override string ToString() => $"{Name} (id:{Id})";
    }
}
