using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Xabbo.Core.Game
{
    public class Room : IRoom
    {
        private readonly ConcurrentDictionary<long, IFloorItem> _floorItems;
        private readonly ConcurrentDictionary<long, IWallItem> _wallItems;
        private readonly ConcurrentDictionary<long, IEntity> _entities;
        private readonly ConcurrentDictionary<int, IEntity> _entityByIndex;
        private readonly ConcurrentDictionary<string, IEntity> _entityByName;

        public long Id { get; }
        public string Model { get; }

        public string Floor { get; set; }
        public string Wallpaper { get; set; }
        public string Landscape { get; set; }

        public RoomData? Data { get; set; }
        IRoomData? IRoom.Data => Data;

        public Tile DoorTile { get; }
        public int EntryDirection { get; }
        public FloorPlan FloorPlan { get; }
        IFloorPlan IRoom.FloorPlan => FloorPlan;
        public Heightmap Heightmap { get; }
        IHeightmap IRoom.Heightmap => Heightmap;

        IEnumerable<IFloorItem> IRoom.FloorItems => _floorItems.Select(x => x.Value);
        IEnumerable<IWallItem> IRoom.WallItems => _wallItems.Select(x => x.Value);
        IEnumerable<IEntity> IRoom.Entities => _entities.Select(x => x.Value);

        public Room(long id, RoomData? data,
            string model, Tile doorTile, int entryDirection, FloorPlan floorPlan, Heightmap heightmap,
            ConcurrentDictionary<long, IFloorItem> floorItems, ConcurrentDictionary<long, IWallItem> wallItems,
            ConcurrentDictionary<long, IEntity> entities, ConcurrentDictionary<int, IEntity> entityByIndex,
            ConcurrentDictionary<string, IEntity> entityByName)
        {
            Id = id;
            Data = data;

            Model = model;
            Floor = string.Empty;
            Wallpaper = string.Empty;
            Landscape = string.Empty;
            DoorTile = doorTile;
            EntryDirection = entryDirection;

            FloorPlan = floorPlan;
            Heightmap = heightmap;

            _floorItems = floorItems;
            _wallItems = wallItems;

            _entities = entities;
            _entityByIndex = entityByIndex;
            _entityByName = entityByName;
        }
    }
}
