using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Xabbo.Core.Game
{
    internal class Room : IRoom
    {
        public long Id { get; }
        public string Model { get; set; } = null!;

        public string? Floor { get; set; }
        public string? Wallpaper { get; set; }
        public string? Landscape { get; set; }

        public RoomData? Data { get; set; }
        IRoomData? IRoom.Data => Data;

        public Tile DoorTile { get; set; }
        public int EntryDirection { get; set; }
        public FloorPlan FloorPlan { get; set; } = null!;
        IFloorPlan IRoom.FloorPlan => FloorPlan;
        public Heightmap Heightmap { get; set; } = null!;
        IHeightmap IRoom.Heightmap => Heightmap;

        IEnumerable<IFloorItem> IRoom.FloorItems => FloorItems.Select(x => x.Value);
        IEnumerable<IWallItem> IRoom.WallItems => WallItems.Select(x => x.Value);
        IEnumerable<IEntity> IRoom.Entities => Entities.Select(x => x.Value);

        internal ConcurrentDictionary<long, FloorItem> FloorItems { get; } = new();
        internal ConcurrentDictionary<long, WallItem> WallItems { get; } = new();

        internal ConcurrentDictionary<long, Entity> Entities { get; } = new();
        internal ConcurrentDictionary<int, Entity> EntityIndexMap { get; } = new();
        internal ConcurrentDictionary<string, Entity> EntityNameMap { get; } = new(StringComparer.OrdinalIgnoreCase);

        public Room(long id)
        {
            Id = id;
        }

        #region - Furni -
        /// <summary>
        /// Gets whether the floor item with the specified ID exists in the room or not.
        /// </summary>
        public bool FloorItemExists(long itemId) => FloorItems.ContainsKey(itemId);

        /// <summary>
        /// Gets whether the wall item with the specified ID exists in the room or not.
        /// </summary>
        public bool WallItemExists(long itemId) => WallItems.ContainsKey(itemId);

        /// <summary>
        /// Attempts to get the furni of the specified type with the specified ID and returns <c>true</c> if successful.
        /// </summary>
        public bool TryGetFurni(ItemType type, long itemId, [NotNullWhen(true)] out IFurni? furni)
            => (furni = GetFurni(type, itemId)) is not null;

        /// <summary>
        /// Attempts to get the floor item with the specified ID and returns <c>true</c> if successful.
        /// </summary>
        public bool TryGetFloorItem(long itemId, [NotNullWhen(true)] out IFloorItem? item)
            => (item = GetFloorItem(itemId)) is not null;

        /// <summary>
        /// Attempts to get the wall item with the specified ID and returns <c>true</c> if successful.
        /// </summary>
        public bool TryGetWallItem(long itemId, [NotNullWhen(true)] out IWallItem? item)
            => (item = GetWallItem(itemId)) is not null;

        /// <summary>
        /// Gets the furni of the specified type with the specified ID, or <c>null</c> if it does not exist.
        /// </summary>
        public IFurni? GetFurni(ItemType type, long itemId)
        {
            return type switch
            {
                ItemType.Floor => GetFloorItem(itemId),
                ItemType.Wall => GetWallItem(itemId),
                _ => throw new InvalidOperationException("Item type must be Floor or Wall")
            };
        }

        /// <summary>
        /// Gets the floor item with the specified ID or <c>null</c> if it does not exist.
        /// </summary>
        public IFloorItem? GetFloorItem(long itemId) => FloorItems.TryGetValue(itemId, out FloorItem? item) ? item : null;

        /// <summary>
        /// Gets the wall item with the specified ID or <c>null</c> if it does not exist.
        /// </summary>
        public IWallItem? GetWallItem(long itemId) => WallItems.TryGetValue(itemId, out WallItem? item) ? item : null;
        #endregion

        #region - Entities -
        
        #endregion
    }
}
