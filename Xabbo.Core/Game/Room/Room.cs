using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Xabbo.Core.Game
{
    internal class Room : IRoom, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public long Id { get; }
        public string Model { get; set; } = null!;

        private string? _floor;
        public string? Floor
        {
            get => _floor;
            set => Set(ref _floor, value);
        }

        private string? _wallpaper;
        public string? Wallpaper
        {
            get => _wallpaper;
            set => Set(ref _wallpaper, value);
        }

        private string? _landscape;
        public string? Landscape
        {
            get => _landscape;
            set => Set(ref _landscape, value);
        }

        private RoomData? _roomData;
        public RoomData? Data
        {
            get => _roomData;
            set => Set(ref _roomData, value);
        }
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

        internal ConcurrentDictionary<int, Entity> Entities { get; } = new();

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
        public TEntity? GetEntity<TEntity>(int index) where TEntity : IEntity
        {
            if (Entities.TryGetValue(index, out Entity? entity) &&
                entity is TEntity typedEntity)
            {
                return typedEntity;
            }
            else
            {
                return default;
            }
        }

        public TEntity? GetEntity<TEntity>(string name) where TEntity : IEntity
        {
            return Entities
                .Select(x => x.Value)
                .OfType<TEntity>()
                .FirstOrDefault(entity => entity.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public TEntity? GetEntityById<TEntity>(long id) where TEntity : IEntity
        {
            return Entities
                .Select(x => x.Value)
                .OfType<TEntity>()
                .FirstOrDefault(entity => entity.Id == id);
        }

        public IEntity? GetEntity(int index) => Entities.TryGetValue(index, out Entity? entity) ? entity : null;
        public IRoomUser? GetUser(int index) => Entities.TryGetValue(index, out Entity? entity) ? (entity as IRoomUser) : null;
        public IRoomUser? GetUserById(long id) => Entities.Values.OfType<IRoomUser>().FirstOrDefault(x => x.Id == id);

        public bool TryGetEntityByIndex(int index, [NotNullWhen(true)] out IEntity? entity) => (entity = GetEntity(index)) is not null;

        public bool TryGetEntityById<TEntity>(long id, [NotNullWhen(true)] out TEntity? entity) where TEntity : IEntity
        {
            return (entity = GetEntityById<TEntity>(id)) is not null;
        }

        public bool TryGetEntityByName<TEntity>(string name, [NotNullWhen(true)] out TEntity? entity) where TEntity : IEntity
        {
            return (entity = GetEntity<TEntity>(name)) is not null;
        }

        public bool TryGetEntityByIndex<TEntity>(int index, [NotNullWhen(true)] out TEntity? entity) where TEntity : IEntity
        {
            if (Entities.TryGetValue(index, out Entity? e))
            {
                if (e is TEntity)
                {
                    entity = (TEntity)(IEntity)e;
                    return true;
                }
            }

            entity = default;
            return false;
        }
        #endregion
    }
}
