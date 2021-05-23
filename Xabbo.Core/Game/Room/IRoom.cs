using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Xabbo.Core.Game
{
    public interface IRoom
    {
        long Id { get; }

        IRoomData? Data { get; }

        string Model { get; }
        string? Floor { get; }
        string? Wallpaper { get; }
        string? Landscape { get; }

        Tile DoorTile { get; }
        int EntryDirection { get; }

        IFloorPlan FloorPlan { get; }
        IHeightmap Heightmap { get; }

        IEnumerable<IFurni> Furni => FloorItems.Concat<IFurni>(WallItems);
        IEnumerable<IFloorItem> FloorItems { get; }
        IEnumerable<IWallItem> WallItems { get; }

        bool FloorItemExists(long id);
        bool WallItemExists(long id);

        #region - Entities -
        IEnumerable<IEntity> Entities { get; }
        IEnumerable<IRoomUser> Users => Entities.OfType<IRoomUser>();
        IEnumerable<IPet> Pets => Entities.OfType<IPet>();
        IEnumerable<IBot> Bots => Entities.OfType<IBot>();

        /// <summary>
        /// Gets the entity with the specified index if it exists.
        /// </summary>
        TEntity? GetEntity<TEntity>(int index) where TEntity : IEntity;
        /// <summary>
        /// Gets the entity with the specified name if it exists.
        /// </summary>
        TEntity? GetEntity<TEntity>(string name) where TEntity : IEntity;
        /// <summary>
        /// Gets the entity with the specified ID if it exists.
        /// </summary>
        TEntity? GetEntityById<TEntity>(long id) where TEntity : IEntity;

        bool TryGetEntityByIndex<TEntity>(int index, [NotNullWhen(true)] out TEntity? entity) where TEntity : IEntity;
        bool TryGetEntityById<TEntity>(long id, [NotNullWhen(true)] out TEntity? entity) where TEntity : IEntity;
        bool TryGetEntityByName<TEntity>(string name, [NotNullWhen(true)] out TEntity? entity) where TEntity : IEntity;

        bool TryGetUserByIndex(int index, [NotNullWhen(true)] out IRoomUser? user) => TryGetEntityByIndex(index, out user);
        bool TryGetUserById(long id, [NotNullWhen(true)] out IRoomUser? user) => TryGetEntityById(id, out user);
        bool TryGetUserByName(string name, [NotNullWhen(true)] out IRoomUser? user) => TryGetEntityByName(name, out user);

        bool TryGetPetByIndex(int index, [NotNullWhen(true)] out IPet? pet) => TryGetEntityByIndex(index, out pet);
        bool TryGetPetById(long id, [NotNullWhen(true)] out IPet? pet) => TryGetEntityById(id, out pet);
        bool TryGetPetByName(string name, [NotNullWhen(true)] out IPet? pet) => TryGetEntityByName(name, out pet);

        bool TryGetBotByIndex(int index, [NotNullWhen(true)] out IBot? bot) => TryGetEntityByIndex(index, out bot);
        bool TryGetBotById(long id, [NotNullWhen(true)] out IBot? bot) => TryGetEntityById(id, out bot);
        bool TryGetBotByName(string name, [NotNullWhen(true)] out IBot? bot) => TryGetEntityByName(name, out bot);
        #endregion
    }
}
