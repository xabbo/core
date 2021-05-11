using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Xabbo.Messages;
using Xabbo.Interceptor;

using Xabbo.Core.Events;
using System.Diagnostics.CodeAnalysis;

namespace Xabbo.Core.Game
{
    /// <summary>
    /// Manages information about the current room, the user's permissions in the room, and its furni and entities.
    /// </summary>
    public class RoomManager : GameStateManager
    {
        private readonly ILogger? _logger;
        private readonly Dictionary<long, RoomData> _roomDataCache = new();

        private Room? _currentRoom;
        private RoomData? _currentRoomData;

        /// <summary>
        /// Gets the ID of the current room. The <see cref="Room"/> may not be available
        /// even when the current room ID is set (e.g. when in the queue).
        /// </summary>
        public long CurrentRoomId { get; private set; }

        public bool IsRingingDoorbell { get; private set; }
        public bool IsInQueue { get; private set; }
        public int QueuePosition { get; private set; }
        // TODO public bool IsSpectating { get; private set; }
        public bool IsLoadingRoom { get; private set; }
        public bool IsInRoom { get; private set; }

        public IRoom? Room { get; private set; }
        public IRoomData? Data => _currentRoomData;

        public int RightsLevel { get; private set; }
        public bool HasRights => RightsLevel > 0;
        public bool IsOwner { get; private set; }

        public bool CanMute => CheckPermission(Room?.Data?.Moderation.WhoCanMute);
        public bool CanKick => CheckPermission(Room?.Data?.Moderation.WhoCanKick);
        public bool CanBan => CheckPermission(Room?.Data?.Moderation.WhoCanBan);

        #region - Room events -
        /// <summary>
        /// Invoked when the user enters the queue to a room.
        /// </summary>
        public event EventHandler? EnteredQueue;
        protected virtual void OnEnteredQueue()
        {
            _logger.LogTrace("Entered queue. (pos:{queuePosition})", QueuePosition);
            EnteredQueue?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invoked when the user's position in the queue is updated.
        /// </summary>
        public event EventHandler? QueuePositionUpdated;
        protected virtual void OnQueuePositionUpdated()
        {
            _logger.LogTrace("Queue position updated. (pos:{queuePosition})", QueuePosition);
            QueuePositionUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invoked when the user enters a room and begins loading the room state.
        /// </summary>
        public event EventHandler? Entering;
        protected virtual void OnEnteringRoom(long roomId)
        {
            _logger.LogTrace("Entering room. (id:{roomId})", roomId);
            Entering?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invoked after the user has entered the room and the room state is fully loaded.
        /// </summary>
        public event EventHandler<RoomEventArgs>? Entered;
        protected virtual void OnEnteredRoom(IRoom room)
        {
            _logger.LogTrace("Entered room. (id:{roomId})", room.Id);
            Entered?.Invoke(this, new RoomEventArgs(room));
        }

        /// <summary>
        /// Invoked when the room data is updated.
        /// </summary>
        public event EventHandler<RoomDataEventArgs>? RoomDataUpdated;
        protected virtual void OnRoomDataUpdated(RoomData roomData)
        {
            _logger.LogTrace("Room data updated. (name:{name})", roomData.Name);
            RoomDataUpdated?.Invoke(this, new RoomDataEventArgs(roomData));
        }

        /// <summary>
        /// Invoked when the user leaves a room.
        /// </summary>
        public event EventHandler? Left;
        protected virtual void OnLeftRoom()
        {
            _logger.LogTrace("Left room.");
            Left?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invoked when the user is kicked from the room.
        /// The user still remains in the room at this point until their avatar leaves through the door.
        /// </summary>
        public event EventHandler? Kicked;
        protected virtual void OnKicked()
        {
            _logger.LogTrace("Kicked from room.");
            Kicked?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invoked when the user's rights to the room are updated.
        /// </summary>
        public event EventHandler? RightsUpdated;
        protected virtual void OnRightsUpdated()
        {
            _logger.LogTrace("Rights updated.");
            RightsUpdated?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region - Furni events -
        /// <summary>
        /// Invoked when the floor items are loaded.
        /// This may happen multiple times depending on
        /// how many items are in the room.
        /// </summary>
        public event EventHandler<FloorItemsEventArgs>? FloorItemsLoaded;
        protected virtual void OnFloorItemsLoaded(IEnumerable<IFloorItem> items)
        {
            _logger.LogTrace("Floor items loaded. ({n})", items.Count());
            FloorItemsLoaded?.Invoke(this, new FloorItemsEventArgs(items));
        }

        /// <summary>
        /// Invoked when a floor item is added to the room.
        /// </summary>
        public event EventHandler<FloorItemEventArgs>? FloorItemAdded;
        protected virtual void OnFloorItemAdded(IFloorItem item)
        {
            _logger.LogTrace("Floor item added. (id:{id})", item.Id);
            FloorItemAdded?.Invoke(this, new FloorItemEventArgs(item));
        }

        /// <summary>
        /// Invoked when a floor item is updated.
        /// </summary>
        public event EventHandler<FloorItemUpdatedEventArgs>? FloorItemUpdated;
        protected virtual void OnFloorItemUpdated(IFloorItem previousItem, IFloorItem updatedItem)
        {
            _logger.LogTrace("Floor item updated. (id:{id})", updatedItem.Id);
            FloorItemUpdated?.Invoke(this, new FloorItemUpdatedEventArgs(previousItem, updatedItem));
        }

        /// <summary>
        /// Invoked when a floor item's data is updated.
        /// </summary>
        public event EventHandler<FloorItemDataUpdatedEventArgs>? FloorItemDataUpdated;
        protected virtual void OnFloorItemDataUpdated(IFloorItem item, IItemData previousData)
        {
            _logger.LogTrace("Floor item data updated. (id:{id})", item.Id);
            FloorItemDataUpdated?.Invoke(this, new FloorItemDataUpdatedEventArgs(item, previousData));
        }

        /// <summary>
        /// Invoked when a floor item slides due to a roller or wired update.
        /// </summary>
        public event EventHandler<FloorItemSlideEventArgs>? FloorItemSlide;
        protected virtual void OnFloorItemSlide(IFloorItem item, Tile previousTile, long rollerId)
        {
            _logger.LogTrace(
                "Floor item slide. (id:{id}, rollerId:{rollerId}, {from} -> {to})",
                item, rollerId, previousTile, item.Location
            );
            FloorItemSlide?.Invoke(this, new FloorItemSlideEventArgs(item, previousTile, rollerId));
        }

        /// <summary>
        /// Invoked when a floor item is removed from the room.
        /// </summary>
        public event EventHandler<FloorItemEventArgs>? FloorItemRemoved;
        protected virtual void OnFloorItemRemoved(IFloorItem item)
        {
            _logger.LogTrace("Floor item removed. (id:{id})", item.Id);
            FloorItemRemoved?.Invoke(this, new FloorItemEventArgs(item));
        }

        /// <summary>
        /// Invoked when the wall items are loaded.
        /// This may happen multiple times depending on
        /// how many items are in the room.
        /// </summary>
        public event EventHandler<WallItemsEventArgs>? WallItemsLoaded;
        protected virtual void OnWallItemsLoaded(IEnumerable<IWallItem> items)
        {
            _logger.LogTrace("Wall items loaded. ({n})", items.Count());
            WallItemsLoaded?.Invoke(this, new WallItemsEventArgs(items));
        }

        /// <summary>
        /// Invoked when a wall item is added to the room.
        /// </summary>
        public event EventHandler<WallItemEventArgs>? WallItemAdded;
        protected virtual void OnWallItemAdded(IWallItem item)
        {
            _logger.LogTrace("Wall item added. (id:{id})", item.Id);
            WallItemAdded?.Invoke(this, new WallItemEventArgs(item));
        }

        /// <summary>
        /// Invoked when a wall item is udpated.
        /// </summary>
        public event EventHandler<WallItemUpdatedEventArgs>? WallItemUpdated;
        protected virtual void OnWallItemUpdated(IWallItem previousItem, IWallItem updatedItem)
        {
            _logger.LogTrace("Wall item updated. (id:{id})", updatedItem.Id);
            WallItemUpdated?.Invoke(this, new WallItemUpdatedEventArgs(previousItem, updatedItem));
        }

        /// <summary>
        /// Invoked when a wall item is removed from the room.
        /// </summary>
        public event EventHandler<WallItemEventArgs>? WallItemRemoved;
        protected virtual void OnWallItemRemoved(IWallItem item)
        {
            _logger.LogTrace("Wall item removed. (id:{id})", item.Id);
            WallItemRemoved?.Invoke(this, new WallItemEventArgs(item));
        }

        /// <summary>
        /// Invoked when a furni's visibility is toggled using <see cref="Hide(IFurni)"/> or <see cref="Show(IFurni)"/>.
        /// </summary>
        public event EventHandler<FurniEventArgs>? FurniVisibilityToggled;
        protected virtual void OnFurniVisibilityToggled(IFurni furni)
        {
            FurniVisibilityToggled?.Invoke(this, new FurniEventArgs(furni));
        }
        #endregion

        #region - Entity events -
        /// <summary>
        /// Invoked when an entity has been added to the room.
        /// </summary>
        public event EventHandler<EntityEventArgs>? EntityAdded;
        protected virtual void OnEntityAdded(IEntity entity)
        {
            EntityAdded?.Invoke(this, new EntityEventArgs(entity));
        }

        /// <summary>
        /// Invoked when entities have been added to the room.
        /// </summary>
        public event EventHandler<EntitiesEventArgs>? EntitiesAdded;
        protected virtual void OnEntitiesAdded(IEnumerable<IEntity> entities)
        {
            _logger.LogTrace("Entities added. ({n})", entities.Count());
            EntitiesAdded?.Invoke(this, new EntitiesEventArgs(entities));
        }

        /// <summary>
        /// Invoked when an entity in the room is updated.
        /// </summary>
        public event EventHandler<EntityEventArgs>? EntityUpdated;
        protected virtual void OnEntityUpdated(IEntity entity)
        {
            EntityUpdated?.Invoke(this, new EntityEventArgs(entity));
        }

        /// <summary>
        /// Invoked when entities in the room are updated.
        /// </summary>
        public event EventHandler<EntitiesEventArgs>? EntitiesUpdated;
        protected virtual void OnEntitiesUpdated(IEnumerable<IEntity> entities)
        {
            _logger.LogTrace("Entities updated. ({n})", entities.Count());
            EntitiesUpdated?.Invoke(this, new EntitiesEventArgs(entities));
        }

        /// <summary>
        /// Invoked when an entity slides along a roller.
        /// </summary>
        public event EventHandler<EntitySlideEventArgs>? EntitySlide;
        protected virtual void OnEntitySlide(IEntity entity, Tile previousTile)
        {
            _logger.LogTrace(
                "Entity slide. ({entityName} [{entityId}:{entityIndex}], {from} -> {to})",
                entity.Name, entity.Id, entity.Index, previousTile, entity.Location
            );
            EntitySlide?.Invoke(this, new EntitySlideEventArgs(entity, previousTile));
        }

        /// <summary>
        /// Invoked when a user's figure, motto or achievement score is updated.
        /// </summary>
        public event EventHandler<UserDataUpdatedEventArgs>? UserDataUpdated;
        protected virtual void OnUserDataUpdated(IRoomUser user,
            string previousFigure, Gender previousGender,
            string previousMotto, int previousAchievementScore)
        {
            _logger.LogTrace(
                "User data updated. ({userName} [{userId}:{userIndex}])",
                user.Name, user.Id, user.Index
            );
            UserDataUpdated?.Invoke(this, new UserDataUpdatedEventArgs(
                user, previousFigure, previousGender,
                previousMotto, previousAchievementScore
            ));
        }

        /// <summary>
        /// Invoked when an entity's name changes.
        /// </summary>
        public event EventHandler<EntityNameChangedEventArgs>? EntityNameChanged;
        protected virtual void OnEntityNameChanged(IEntity entity, string previousName)
        {
            _logger.LogTrace(
                "Entity name changed. ({previousName} -> {entityName} [{entityId}:{entityIndex}])",
                previousName, entity.Name, entity.Id, entity.Index
            );
            EntityNameChanged?.Invoke(this, new EntityNameChangedEventArgs(entity, previousName));
        }

        /// <summary>
        /// Invoked when an entity's idle status updates.
        /// </summary>
        public event EventHandler<EntityIdleEventArgs>? EntityIdle;
        protected virtual void OnEntityIdle(IEntity entity, bool wasIdle)
        {
            _logger.LogTrace(
                "Entity idle. ({entityName} [{entityId}:{entityIndex}], {isIdle} -> {wasIdle})",
                entity.Name, entity.Id, entity.Index, entity.IsIdle, wasIdle
            );
            EntityIdle?.Invoke(this, new EntityIdleEventArgs(entity, wasIdle));
        }

        /// <summary>
        /// Invoked when an entity's dance updates.
        /// </summary>
        public event EventHandler<EntityDanceEventArgs>? EntityDance;
        protected virtual void OnEntityDance(IEntity entity, int previousDance)
        {
            _logger.LogTrace(
                "Entity dance. ({entityName} [{entityId}:{entityIndex}], {dance} -> {previousDance})",
                entity.Name, entity.Id, entity.Index, entity.Dance, previousDance
            );
            EntityDance?.Invoke(this, new EntityDanceEventArgs(entity, previousDance));
        }

        /// <summary>
        /// Invoked when an entity's hand item updates.
        /// </summary>
        public event EventHandler<EntityHandItemEventArgs>? EntityHandItem;
        protected virtual void OnEntityHandItem(IEntity entity, int previousItem)
        {
            _logger.LogTrace(
                "Entity hand item. ({entityName} [{entityId}:{entityIndex}], {itemId} -> {previousItemId})",
                entity.Name, entity.Id, entity.Index, entity.HandItem, previousItem
            );
            EntityHandItem?.Invoke(this, new EntityHandItemEventArgs(entity, previousItem));
        }

        /// <summary>
        /// Invoked when an entity's effect updates.
        /// </summary>
        public event EventHandler<EntityEffectEventArgs>? EntityEffect;
        protected virtual void OnEntityEffect(IEntity entity, int previousEffect)
        {
            _logger.LogTrace(
                "Entity effect. ({entityName} [{entityId}:{entityIndex}], {effect} -> {previousEffect})",
                entity.Name, entity.Id, entity.Index, entity.Effect, previousEffect
            );
            EntityEffect?.Invoke(this, new EntityEffectEventArgs(entity, previousEffect));
        }

        /// <summary>
        /// Invoked when an entity performs an action.
        /// </summary>
        public event EventHandler<EntityExpressionEventArgs>? EntityExpression;
        protected virtual void OnEntityExpression(IEntity entity, Expressions expression)
        {
            _logger.LogTrace(
                "Entity expression. ({entityName} [{entityId}:{entityIndex}], action:{action})",
                entity.Name, entity.Id, entity.Index, expression
            );
            EntityExpression?.Invoke(this, new EntityExpressionEventArgs(entity, expression));
        }

        /// <summary>
        /// Invoked when an entity's typing status updates.
        /// </summary>
        public event EventHandler<EntityTypingEventArgs>? EntityTyping;
        protected virtual void OnEntityTyping(IEntity entity, bool wasTyping)
        {
            _logger.LogTrace(
                "Entity typing. ({entityName} [{entityId}:{entityIndex}], {isTyping} -> {wasTyping})",
                entity.Name, entity.Id, entity.Index, entity.IsTyping, wasTyping
            );
            EntityTyping?.Invoke(this, new EntityTypingEventArgs(entity, wasTyping));
        }

        /// <summary>
        /// Invoked when an entity is removed from the room.
        /// </summary>
        public event EventHandler<EntityEventArgs>? EntityRemoved;
        protected virtual void OnEntityRemoved(IEntity entity)
        {
            _logger.LogTrace(
                "Entity removed. ({entityName} [{entityId}:{entityIndex}])",
                entity.Name, entity.Id, entity.Index
            );
            EntityRemoved?.Invoke(this, new EntityEventArgs(entity));
        }
        #endregion

        /// <summary>
        /// Retrieves the room data from the cache if it is available.
        /// </summary>
        public bool TryGetRoomData(long roomId, [NotNullWhen(true)] out RoomData? data) => _roomDataCache.TryGetValue(roomId, out data);

#pragma warning disable CS8618 // Non-nullable fields are initialized in the ResetRoom() method.
        public RoomManager(ILogger<RoomManager>? logger, IInterceptor interceptor)
            : base(interceptor)
        {
            _logger = logger;

            ResetState();
        }

        public RoomManager(IInterceptor interceptor)
            : this(null, interceptor)
        { }
#pragma warning restore CS8618

        protected override void OnInterceptorDisconnected(object? sender, EventArgs e)
        {
            base.OnInterceptorDisconnected(sender, e);


        }

        #region - Room packet handlers -
        private void ResetState() 
        {
            IsInQueue =
            IsLoadingRoom =
            IsInRoom = false;
            QueuePosition = 0;

            RightsLevel = 0;
            IsOwner = false;

            _currentRoom = null;
            _currentRoomData = null;

            Room = null;
        }

        private bool CheckPermission(ModerationPermissions? permissions)
        {
            if (!IsInRoom) return false;

            return permissions switch
            {
                ModerationPermissions.OwnerOnly => IsOwner,
                ModerationPermissions.RightsHolders => RightsLevel > 0,
                ModerationPermissions.AllUsers => true,
                _ => false,
            };
        }

        private void SetFurniHidden(ItemType type, long id, bool hide)
        {
            if (_currentRoom is null) return;

            Furni furni;

            if (type == ItemType.Floor)
            {
                if (!_currentRoom.FloorItems.TryGetValue(id, out FloorItem? item))
                    return;

                if (item.IsHidden == hide) return;

                item.IsHidden = hide;
                furni = _currentRoom.FloorItems.AddOrUpdate(
                    id,
                    item,
                    (key, existing) =>
                    {
                        existing.IsHidden = hide;
                        return existing;
                    }
                );
            }
            else if (type == ItemType.Wall)
            {
                if (!_currentRoom.WallItems.TryGetValue(id, out WallItem? item))
                    return;

                if (item.IsHidden == hide) return;

                item.IsHidden = hide;
                furni = _currentRoom.WallItems.AddOrUpdate(
                    id,
                    item,
                    (key, existing) =>
                    {
                        existing.IsHidden = hide;
                        return existing;
                    }
                );
            }
            else
            {
                return;
            }

            if (hide)
            {
                if (furni.Type == ItemType.Floor)
                {
                    SendClientAsync(In.ActiveObjectRemove, furni.Id, false, -1L, 0);
                }
                else if (furni.Type == ItemType.Wall)
                {
                    SendClientAsync(In.RemoveItem, furni.Id, -1L);
                }
            }
            else
            {
                SendClientAsync(furni.Type == ItemType.Floor ? In.ActiveObjectAdd : In.AddItem , furni);
            }

            OnFurniVisibilityToggled(furni);
        }

        public void ShowFurni(ItemType type, int id) => SetFurniHidden(type, id, false);
        public void HideFurni(ItemType type, int id) => SetFurniHidden(type, id, true);

        [Receive(nameof(Incoming.GetGuestRoomResult))]
        private void HandleGetGuestRoomResult(IReadOnlyPacket packet)
        {
            var roomData = RoomData.Parse(packet);

            if (!_roomDataCache.ContainsKey(roomData.Id))
            {
                _logger.LogTrace(
                    "Storing room data in cache. (id:{roomId}, name:'{roomName}', owner:'{roomOwner}')",
                    roomData.Id,
                    roomData.Name,
                    roomData.OwnerName
                );
            }
            else
            {
                _logger.LogTrace("Updating room data cache. (id:{roomId}, name:'{roomName}', owner:'{roomOwner}')",
                    roomData.Id,
                    roomData.Name,
                    roomData.OwnerName
                );
            }

            _roomDataCache[roomData.Id] = roomData;

            if (IsInRoom && roomData.Id == _currentRoom?.Id)
            {
                _currentRoomData = roomData;
                if (_currentRoom is not null)
                    _currentRoom.Data = roomData;

                OnRoomDataUpdated(roomData);
            }
        }

        [Receive(nameof(Incoming.OpenConnectionConfirmation))]
        private void HandleOpenConnectionConfirmation(IReadOnlyPacket packet)
        {
            if (IsInQueue || IsLoadingRoom || IsInRoom)
            {
                ResetState();
                OnLeftRoom();
            }

            long roomId = packet.ReadLegacyLong();
            _currentRoom = new Room(roomId);

            if (_roomDataCache.TryGetValue(roomId, out RoomData? data))
            {
                _currentRoomData = data;
                _logger.LogTrace("Loaded room data from cache.");
            }
        }

        [Receive(nameof(Incoming.RoomQueueStatus))]
        private void HandleRoomQueueStatus(IReadOnlyPacket packet)
        {
            long roomId = packet.ReadLegacyLong();
            if (roomId != _currentRoom?.Id) return;

            if (packet.ReadShort() == 2 &&
                packet.ReadString() == "visitors" &&
                packet.ReadInt() == 2 &&
                packet.ReadShort() == 1 &&
                packet.ReadString() == "visitors")
            {
                bool enteredQueue = !IsInQueue;

                IsInQueue = true;
                QueuePosition = packet.ReadInt() + 1;

                if (enteredQueue)
                {
                    OnEnteredQueue();
                }
                else
                {
                    OnQueuePositionUpdated();
                }
            }
        }

        [Receive(nameof(Incoming.RoomReady))]
        private void HandleRoomReady(IReadOnlyPacket packet)
        {
            if (IsInQueue)
            {
                IsInQueue = false;
                QueuePosition = 0;
            }
            else if (IsLoadingRoom || IsInRoom)
            {
                ResetState();
                OnLeftRoom();
            }

            string model = packet.ReadString();
            long roomId = packet.ReadLegacyLong();

            if (_currentRoom is null)
            {
                _currentRoom = new Room(roomId);
            }

            _currentRoom.Model = model;
            IsLoadingRoom = true;

            OnEnteringRoom(_currentRoom.Id);
        }

        [Receive(nameof(Incoming.FlatProperty))]
        private void HandleFlatProperty(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom)
            {
                _logger.LogWarning($"[{nameof(HandleFlatProperty)}] Not loading room!");
                return;
            }

            if (_currentRoom is null) 
            {
                _logger.LogTrace($"[{nameof(HandleFlatProperty)}] Current room is null.");
                return;
            }

            string key = packet.ReadString();
            string value = packet.ReadString();
            switch (key)
            {
                case "floor": _currentRoom.Floor = value; break;
                case "wallpaper": _currentRoom.Wallpaper = value; break;
                case "landscape": _currentRoom.Landscape = value; break;
                default: _logger.LogDebug("Unknown paint type: {type}.", key); break;
            }

            // TODO Add a property updated event if already in room
        }

        [Receive(nameof(Incoming.YouAreController))]
        private void HandleYouAreController(IReadOnlyPacket packet)
        {
            if (_currentRoom is null) 
            {
                _logger.LogTrace($"[{nameof(HandleYouAreController)}] Current room is null.");
                return;
            }

            long roomId = packet.ReadLegacyLong();
            if (roomId != _currentRoom.Id) 
            {
                _logger.LogWarning($"[{nameof(HandleYouAreController)}] Room ID mismatch!");
                return;
            }

            RightsLevel = packet.ReadInt();
            OnRightsUpdated();
        }

        [Receive(nameof(Incoming.YouAreNotController))]
        private void HandleRoomNoRights(IReadOnlyPacket packet)
        {
            if (_currentRoom is null) 
            {
                _logger.LogTrace($"[{nameof(HandleRoomNoRights)}] Current room is null.");
                return;
            }

            long roomId = packet.ReadLegacyLong();
            if (roomId != _currentRoom.Id) 
            {
                _logger.LogWarning($"[{nameof(HandleRoomNoRights)}] Room ID mismatch!");
                return;
            }

            RightsLevel = 0;
            OnRightsUpdated();
        }

        [Receive(nameof(Incoming.RoomEntryTile))]
        private void HandleRoomEntryTile(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom) 
            {
                _logger.LogWarning($"[{nameof(HandleRoomEntryTile)}] Not loading room!");
                return;
            }

            if (_currentRoom is null) 
            {
                _logger.LogTrace($"[{nameof(HandleRoomEntryTile)}] Current room is null.");
                return;
            }

            int x = packet.ReadInt();
            int y = packet.ReadInt();
            int dir = packet.ReadInt();

            _currentRoom.DoorTile = new Tile(x, y, 0);
            _currentRoom.EntryDirection = dir;

            _logger.LogTrace("Received room entry tile. (x:{x}, y:{y}, dir: {dir})", x, y, dir);
        }

        [Receive(nameof(Incoming.StackingHeightmap))]
        private void HandleStackingHeightmap(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom) 
            {
                _logger.LogWarning($"[{nameof(HandleStackingHeightmap)}] Not loading room!");
                return;
            }

            if (_currentRoom is null) 
            {
                _logger.LogTrace($"[{nameof(HandleStackingHeightmap)}] Current room is null.");
                return;
            }

            Heightmap heightmap = Heightmap.Parse(packet);
            _currentRoom.Heightmap = heightmap;

            _logger.LogTrace("Received stacking heightmap. (size:{width}x{length})", heightmap.Width, heightmap.Length);
        }

        [Receive(nameof(Incoming.StackingHeightmapDiff))]
        private void HandleStackingHeightmapDiff(IReadOnlyPacket packet)
        {
            if (_currentRoom is null) 
            {
                _logger.LogTrace($"[{nameof(HandleStackingHeightmapDiff)}] Current room is null.");
                return;
            }

            int n = packet.ReadByte();
            for (int i = 0; i < n; i++)
            {
                int x = packet.ReadByte();
                int y = packet.ReadByte();
                _currentRoom.Heightmap[x, y] = packet.ReadShort();
            }

            _logger.LogTrace("Received stacking heightmap diff. ({n} changes)", n);
        }

        [Receive(nameof(Incoming.FloorHeightmap))]
        private void HandleFloorHeightmap(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom)
            {
                _logger.LogWarning($"[{nameof(HandleFloorHeightmap)}] Not loading room!");
                return;
            }

            if (_currentRoom is null) 
            {
                _logger.LogTrace($"[{nameof(HandleFloorHeightmap)}] Current room is null.");
                return;
            }

            FloorPlan floorPlan = FloorPlan.Parse(packet);
            _currentRoom.FloorPlan = floorPlan;

            _logger.LogTrace("Received floor heightmap. (size:{width}x{length})", floorPlan.Width, floorPlan.Length);
        }

        [Receive(nameof(Incoming.RoomEntryInfo))]
        private void HandleRoomEntryInfo(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom)
            {
                _logger.LogWarning($"[{nameof(HandleRoomEntryInfo)}] Not loading room!");
                return;
            }

            long roomId = packet.ReadLegacyLong();
            if (roomId != _currentRoom?.Id)
            {
                _logger.LogWarning(
                    "Room ID mismatch! (expected:{expectedRoomId}, actual:{actualRoomId})",
                    _currentRoom?.Id, roomId
                );
                return;
            }

            if (_roomDataCache.TryGetValue(roomId, out RoomData? roomData))
            {
                _logger.LogTrace("Loaded room data from cache.");
                _currentRoomData = roomData;
                _currentRoom.Data = roomData;
            }

            // if (_floorPlan is null ||
            //     _heightmap is null ||
            //     _floorItems is null ||
            //     _wallItems is null ||
            //     _entities is null ||
            //     _entityIndexMap is null ||
            //     _entityNameMap is null)
            // {
            //     _logger.LogError("");
            //     return;
            // }

            // _currentRoom = new Room(_currentRoomId);

            IsOwner = packet.ReadBool();
            IsLoadingRoom = false;
            IsInRoom = true;
            Room = _currentRoom;

            OnEnteredRoom(_currentRoom);
        }

        [Receive(nameof(Incoming.CloseConnection))]
        private void HandleCloseConnection(IReadOnlyPacket packet)
        {
            if (_currentRoom is null) 
            {
                _logger.LogTrace($"[{nameof(HandleCloseConnection)}] Current room is null.");
                return;
            }

            if (IsRingingDoorbell || IsInQueue || IsLoadingRoom || IsInRoom)
            {
                ResetState();
                OnLeftRoom();
            }
        }

        [Receive(nameof(Incoming.Error))]
        private void HandleError(IReadOnlyPacket packet)
        {
            if (!IsInRoom) return;

            ErrorCode errorCode = (ErrorCode)packet.ReadInt();
            if (errorCode == ErrorCode.Kicked)
                OnKicked();
        }
        #endregion

        /*#region - Floor item packet handlers -
        [Receive(nameof(Incoming.ActiveObjects))]
        protected void HandleActiveObjects(IReadOnlyPacket packet)
        {
            Room? room = _roomManager.Room as Room;
            if (!_roomManager.IsLoadingRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            List<FloorItem> newItems = new List<FloorItem>();

            FloorItem[] items = FloorItem.ParseAll(packet);
            foreach (FloorItem item in items)
            {
                if (_floorItems.TryAdd(item.Id, item))
                {
                    newItems.Add(item);
                }
                else
                {
                    DebugUtil.Log($"failed to add item {item.Id} to the dictionary");
                }
            }

            if (newItems.Count > 0)
            {
                DebugUtil.Log($"loaded {newItems.Count} items");
                OnFloorItemsLoaded(newItems);
            }
        }

        [Receive(nameof(Incoming.ActiveObjectAdd))]
        protected void HandleAddFloorItem(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
                return;

            FloorItem item = FloorItem.Parse(packet);

            if (_floorItems.TryAdd(item.Id, item))
            {
                OnFloorItemAdded(item);
            }
            else
            {
                DebugUtil.Log($"failed to add item {item.Id} to the dictionary");
            }
        }

        [Receive(nameof(Incoming.ActiveObjectRemove))]
        protected void HandleActiveObjectRemove(IReadOnlyPacket packet)
        {
            *//*
                long id
                bool isExpired
                long pickerId
                int delay
            *//*

            if (!_roomManager.IsInRoom)
                return;

            long id = packet.ReadLong();

            if (_floorItems.TryRemove(id, out FloorItem? item))
            {
                OnFloorItemRemoved(item);
            }
            else
            {
                DebugUtil.Log($"failed to remove item {id} from the dictionary");
            }
        }

        [Receive(nameof(Incoming.ActiveObjectUpdate))]
        protected void HandleActiveObjectUpdate(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
                return;

            var updatedItem = FloorItem.Parse(packet);

            if (_floorItems.TryGetValue(updatedItem.Id, out FloorItem? previousItem))
            {
                updatedItem.OwnerName = previousItem.OwnerName;
                updatedItem.IsHidden = previousItem.IsHidden;

                if (_floorItems.TryUpdate(updatedItem.Id, updatedItem, previousItem))
                {
                    OnFloorItemUpdated(previousItem, updatedItem);
                }
                else
                {
                    DebugUtil.Log($"failed to update item {updatedItem.Id}");
                }
            }
            else
            {
                DebugUtil.Log($"unable to find item {updatedItem.Id} to update");
            }
        }

        [Receive(nameof(Incoming.QueueMoveUpdate))]
        protected void HandleObjectOnRoller(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
                return;

            var rollerUpdate = RollerUpdate.Parse(packet);
            foreach (var objectUpdate in rollerUpdate.ObjectUpdates)
            {
                if (_floorItems.TryGetValue(objectUpdate.Id, out FloorItem? item))
                {
                    var previousTile = item.Location;
                    item.Location = new Tile(rollerUpdate.TargetX, rollerUpdate.TargetY, objectUpdate.TargetZ);
                    OnFloorItemSlide(item, previousTile, rollerUpdate.RollerId);
                }
                else
                {
                    DebugUtil.Log($"unable to find floor item {objectUpdate.Id} to update");
                }
            }
        }

        [Receive(nameof(Incoming.StuffDataUpdate))]
        protected void HandleItemExtraData(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
                return;

            long id = packet.ReadLong();

            if (!_floorItems.TryGetValue(id, out FloorItem? item))
            {
                DebugUtil.Log($"unable to find floor item {id} to update");
                return;
            }

            var previousData = item.Data;
            item.Data = StuffData.Parse(packet);

            OnFloorItemDataUpdated(item, previousData);
        }

        [Receive(nameof(Incoming.MultipleStuffDataUpdate))]
        protected void HandleItemsDataUpdate(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
                return;

            int n = packet.ReadShort();
            for (int i = 0; i < n; i++)
            {
                long itemId = packet.ReadLong();
                var data = StuffData.Parse(packet);
                if (!_floorItems.TryGetValue(itemId, out FloorItem? item)) continue;

                var previousData = item.Data;
                item.Data = data;

                OnFloorItemDataUpdated(item, previousData);
            }
        }
        #endregion*/

        /*#region - Wall item packet handlers -
        [Receive(nameof(Incoming.Items))]
        protected void HandleItems(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            List<WallItem> newItems = new List<WallItem>();

            WallItem[] items = WallItem.ParseAll(packet);
            foreach (var item in items)
            {
                if (_wallItems.TryAdd(item.Id, item))
                {
                    newItems.Add(item);
                }
                else
                {
                    DebugUtil.Log($"failed to add wall item with ID {item.Id}");
                }
            }

            if (newItems.Count > 0)
            {
                DebugUtil.Log($"loaded {newItems.Count} items");
                OnWallItemsLoaded(newItems);
            }
        }

        [Receive(nameof(Incoming.AddItem))]
        protected void HandleAddItem(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
                return;

            var item = WallItem.Parse(packet);
            if (_wallItems.TryAdd(item.Id, item))
            {
                OnWallItemAdded(item);
            }
            else
            {
                DebugUtil.Log($"failed to add item {item.Id} to the dictionary");
            }
        }

        [Receive(nameof(Incoming.RemoveItem))]
        protected void HandleRemoveItem(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
                return;

            long id = packet.ReadLong();
            // long pickerId

            if (_wallItems.TryRemove(id, out WallItem? item))
            {
                OnWallItemRemoved(item);
            }
            else
            {
                DebugUtil.Log($"failed to remove item {id} from the dictionary");
            }
        }

        [Receive(nameof(Incoming.UpdateItem))]
        protected void HandleUpdateItem(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
                return;
            
            var updatedItem = WallItem.Parse(packet);
            WallItem? previousItem = null;

            updatedItem = _wallItems.AddOrUpdate(
                updatedItem.Id,
                updatedItem,
                (id, existing) =>
                {
                    previousItem = existing;
                    updatedItem.OwnerName = previousItem.OwnerName;
                    updatedItem.IsHidden = previousItem.IsHidden;
                    return updatedItem;
                }
            );

            if (previousItem == null)
            {
                DebugUtil.Log($"failed to find previous wall item for update {updatedItem.Id}");
            }
            else
            {
                OnWallItemUpdated(previousItem, updatedItem);
            }

            *//*
            if (wallItems.TryGetValue(updatedItem.Id, out WallItem previousItem))
            {
                updatedItem.OwnerName = previousItem.OwnerName;
                updatedItem.IsHidden = previousItem.IsHidden;

                if (wallItems.TryUpdate(updatedItem.Id, updatedItem, previousItem))
                {
                    OnWallItemUpdated(previousItem, updatedItem);
                }
                else
                {
                    DebugUtil.Log($"failed to update wall item {updatedItem.Id}");
                }
            }
            else
            {
                DebugUtil.Log($"failed to find wall item {updatedItem.Id} to update");
            }*//*
        }
        #endregion

        #region - Entity packet handlers -

        #endregion*/
    }
}
