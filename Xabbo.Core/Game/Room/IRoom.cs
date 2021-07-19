using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Xabbo.Core.Game
{
    public interface IRoom
    {
        long Id { get; }

        IRoomData Data { get; }

        #region - Room data -
        string Name => Data.Name;
        string Description => Data.Description;
        long OwnerId => Data.OwnerId;
        string OwnerName => Data.OwnerName;

        RoomAccess Access => Data.Access;
        bool IsOpen => Access == RoomAccess.Open;
        bool IsDoorbell => Access == RoomAccess.Doorbell;
        bool IsLocked => Access == RoomAccess.Password;
        bool IsInvisible => Access == RoomAccess.Invisible;

        int MaxUsers => Data.MaxUsers;
        TradePermissions Trading => Data.Trading;
        int Score => Data.Score;
        int Ranking => Data.Ranking;
        RoomCategory Category => Data.Category;
        IReadOnlyList<string> Tags => Data.Tags;

        RoomFlags Flags => Data.Flags;
        bool HasEvent => Data.HasEvent;
        bool IsGroupRoom => Data.IsGroupRoom;
        bool AllowPets => Data.AllowPets;

        long GroupId => Data.GroupId;
        string GroupName => Data.GroupName;
        string GroupBadge => Data.GroupBadge;

        string EventName => Data.EventName;
        string EventDescription => Data.EventDescription;
        int EventMinutesRemaining => Data.EventMinutesRemaining;

        IModerationSettings Moderation => Data.Moderation;
        IChatSettings ChatSettings => Data.ChatSettings;
        #endregion

        string Model { get; }
        string? Floor { get; }
        string? Wallpaper { get; }
        string? Landscape { get; }

        Tile DoorTile { get; }
        int EntryDirection { get; }

        IFloorPlan FloorPlan { get; }
        IHeightmap Heightmap { get; }

        #region - Furni -
        IEnumerable<IFurni> Furni => FloorItems.Concat<IFurni>(WallItems);
        IEnumerable<IFloorItem> FloorItems { get; }
        IEnumerable<IWallItem> WallItems { get; }

        bool FloorItemExists(long id);
        bool WallItemExists(long id);

        IFurni? GetFurni(ItemType type, long id);
        IFloorItem? GetFloorItem(long id);
        IWallItem? GetWallItem(long id);
        #endregion

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
