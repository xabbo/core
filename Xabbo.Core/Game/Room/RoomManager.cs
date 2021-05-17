using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Microsoft.Extensions.Logging;

using Xabbo.Messages;
using Xabbo.Interceptor;

using Xabbo.Core.Events;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging.Abstractions;

namespace Xabbo.Core.Game
{
    /// <summary>
    /// Manages information about the current room, the user's permissions in the room, its furni, entities and chat.
    /// </summary>
    public class RoomManager : GameStateManager
    {
        private readonly ILogger _logger;
        private readonly Dictionary<long, RoomData> _roomDataCache = new();

        private Room? _currentRoom;
        private RoomData? _currentRoomData;

        /// <summary>
        /// Gets the ID of the current room. The <see cref="Room"/> may not be available
        /// even when the current room ID is set (e.g. when in the queue).
        /// </summary>
        private long _currentRoomId;
        public long CurrentRoomId
        {
            get => _currentRoomId;
            private set => Set(ref _currentRoomId, value);
        }

        private bool _isRingingDoorbell;
        public bool IsRingingDoorbell
        {
            get => _isRingingDoorbell;
            private set => Set(ref _isRingingDoorbell, value);
        }

        private bool _isInQueue;
        public bool IsInQueue
        {
            get => _isInQueue;
            private set => Set(ref _isInQueue, value);
        }

        private int _queuePosition;
        public int QueuePosition
        {
            get => _queuePosition;
            private set => Set(ref _queuePosition, value);
        }

        private bool _isSpectating;
        public bool IsSpectating
        {
            get => _isSpectating;
            set => Set(ref _isSpectating, value);
        }

        private bool _isLoadingRoom;
        public bool IsLoadingRoom
        {
            get => _isLoadingRoom;
            private set => Set(ref _isLoadingRoom, value);
        }

        private bool _isInRoom;
        public bool IsInRoom
        {
            get => _isInRoom;
            private set => Set(ref _isInRoom, value);
        }

        private IRoom? _room;
        public IRoom? Room
        {
            get => _room;
            set => Set(ref _room, value);
        }

        private IRoomData? _roomData;
        public IRoomData? Data
        {
            get => _roomData;
            set => Set(ref _roomData, value);
        }

        private int _rightsLevel;
        public int RightsLevel
        {
            get => _rightsLevel;
            private set
            {
                if (Set(ref _rightsLevel, value))
                    RaisePropertyChanged(nameof(HasRights));
            }
        }
        public bool HasRights => RightsLevel > 0;

        private bool _isOwner;
        public bool IsOwner
        {
            get => _isOwner;
            private set => Set(ref _isOwner, value);
        }

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
                item.Id, rollerId, previousTile, item.Location
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

        /// <summary>
        /// Invoked when an entity in the room talks.
        /// </summary>
        public event EventHandler<EntityChatEventArgs>? EntityChat;
        protected virtual void OnEntityChat(EntityChatEventArgs e)
        {
            _logger.LogTrace(
                "{type}:{bubble} {entity} {message}",
                e.ChatType, e.BubbleStyle, e.Entity, e.Message
            );
            EntityChat?.Invoke(this, e);
        }
        #endregion

        /// <summary>
        /// Retrieves the room data from the cache if it is available.
        /// </summary>
        public bool TryGetRoomData(long roomId, [NotNullWhen(true)] out RoomData? data) => _roomDataCache.TryGetValue(roomId, out data);

#pragma warning disable CS8618 // Non-nullable fields are initialized in the ResetRoom() method.
        public RoomManager(ILogger<RoomManager> logger, IInterceptor interceptor)
            : base(interceptor)
        {
            _logger = logger;

            ResetState();
        }

        public RoomManager(IInterceptor interceptor)
            : base(interceptor)
        {
            _logger = NullLogger.Instance;

            ResetState();
        }
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

            Room = _currentRoom = null;
            Data = _currentRoomData = null;
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
                    Send(In.ActiveObjectRemove, furni.Id, false, -1L, 0);
                }
                else if (furni.Type == ItemType.Wall)
                {
                    Send(In.RemoveItem, furni.Id, -1L);
                }
            }
            else
            {
                Send(furni.Type == ItemType.Floor ? In.ActiveObjectAdd : In.AddItem , furni);
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
                Set(ref _currentRoomData, roomData, nameof(Data));
                if (_currentRoom is not null)
                {
                    _currentRoom.Data = roomData;
                }

                OnRoomDataUpdated(roomData);
            }
        }

        [Receive(nameof(Incoming.OpenConnectionConfirmation))]
        private void HandleOpenConnectionConfirmation(IReadOnlyPacket packet)
        {
            if (IsInQueue || IsRingingDoorbell || IsLoadingRoom || IsInRoom)
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

        [Receive(nameof(Incoming.YouAreSpectator))]
        private void HandleYouAreSpectator(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom)
            {
                _logger.LogDebug("Not loading room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            long roomId = packet.ReadLegacyLong();
            if (roomId != _currentRoom.Id)
            {
                _logger.LogError("Room ID mismatch!");
                return;
            }

            IsSpectating = true;
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

            if (_currentRoom is null ||
                _currentRoom.Id != roomId)
            {
                _currentRoom = new Room(roomId);
            }

            _currentRoom.Model = model;
            IsLoadingRoom = true;

            OnEnteringRoom(roomId);
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

            if (IsInRoom)
            {
                // TODO PropertyUpdated
            }
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

            _logger.LogTrace("Received stacking heightmap diff. ({n} change(s))", n);
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
                _logger.LogWarning($"[{nameof(HandleCloseConnection)}] Current room is null.");
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

        #region - Floor item packet handlers -
        [Receive(nameof(Incoming.ActiveObjects))]
        protected void HandleActiveObjects(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom)
            {
                _logger.LogDebug("Not loading room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            List<FloorItem> newItems = new List<FloorItem>();

            FloorItem[] items = FloorItem.ParseAll(packet);
            foreach (FloorItem item in items)
            {
                if (_currentRoom.FloorItems.TryAdd(item.Id, item))
                {
                    newItems.Add(item);
                }
                else
                {
                    _logger.LogError("Failed to add floor item {itemId}.", item.Id);
                }
            }

            if (newItems.Count > 0)
            {
                OnFloorItemsLoaded(newItems);
            }
        }

        [Receive(nameof(Incoming.ActiveObjectAdd))]
        protected void HandleAddFloorItem(IReadOnlyPacket packet)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug($"[{nameof(HandleAddFloorItem)}] Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning($"[{nameof(HandleAddFloorItem)}] Current room is null.");
                return;
            }

            FloorItem item = FloorItem.Parse(packet);

            if (_currentRoom.FloorItems.TryAdd(item.Id, item))
            {
                OnFloorItemAdded(item);
            }
            else
            {
                _logger.LogError($"[{nameof(HandleAddFloorItem)}] Failed to add floor item {{itemId}}.", item.Id);
            }
        }

        [Receive(nameof(Incoming.ActiveObjectRemove))]
        protected void HandleActiveObjectRemove(IReadOnlyPacket packet)
        {
            /*
                long id
                bool isExpired
                long pickerId
                int delay
            */

            if (!IsInRoom)
            {
                _logger.LogDebug("[{method}] Not in room.", nameof(HandleActiveObjectRemove));
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("[{method}] Current room is null.", nameof(HandleActiveObjectRemove));
                return;
            }

            long id = packet.Protocol switch
            {
                ClientType.Flash => long.Parse(packet.ReadString()),
                ClientType.Unity => packet.ReadLong(),
                _ => throw new InvalidOperationException("Unknown protocol type")
            };

            if (_currentRoom.FloorItems.TryRemove(id, out FloorItem? item))
            {
                OnFloorItemRemoved(item);
            }
            else
            {
                _logger.LogError("[{method}] Failed to remove item {id} from the dictionary", nameof(HandleActiveObjectRemove), id);
            }
        }

        [Receive(nameof(Incoming.ActiveObjectUpdate))]
        protected void HandleActiveObjectUpdate(IReadOnlyPacket packet)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("[{method}] Not in room.", nameof(HandleActiveObjectUpdate));
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("[{method}] Current room is null.", nameof(HandleActiveObjectUpdate));
                return;
            }

            var updatedItem = FloorItem.Parse(packet);

            if (_currentRoom.FloorItems.TryGetValue(updatedItem.Id, out FloorItem? previousItem))
            {
                updatedItem.OwnerName = previousItem.OwnerName;
                updatedItem.IsHidden = previousItem.IsHidden;

                if (_currentRoom.FloorItems.TryUpdate(updatedItem.Id, updatedItem, previousItem))
                {
                    OnFloorItemUpdated(previousItem, updatedItem);
                }
                else
                {
                    _logger.LogError("[{method}] Failed to update floor item {itemId}", nameof(HandleActiveObjectUpdate), updatedItem.Id);
                }
            }
            else
            {
                _logger.LogError("[{method}] Failed to find floor item {itemId} to update", nameof(HandleActiveObjectUpdate), updatedItem.Id);
            }
        }

        [Receive(nameof(Incoming.QueueMoveUpdate))]
        protected void HandleObjectOnRoller(IReadOnlyPacket packet)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("[{method}] Not in room.", nameof(HandleObjectOnRoller));
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("[{method}] Current room is null.", nameof(HandleObjectOnRoller));
                return;
            }

            RollerUpdate rollerUpdate = RollerUpdate.Parse(packet);
            foreach (RollerObjectUpdate objectUpdate in rollerUpdate.ObjectUpdates)
            {
                if (_currentRoom.FloorItems.TryGetValue(objectUpdate.Id, out FloorItem? item))
                {
                    Tile previousTile = item.Location;
                    item.Location = new Tile(rollerUpdate.TargetX, rollerUpdate.TargetY, objectUpdate.TargetZ);
                    OnFloorItemSlide(item, previousTile, rollerUpdate.RollerId);
                }
                else
                {
                    _logger.LogError("[{method}] Failed to find floor item {itemId} to update.", nameof(HandleObjectOnRoller), objectUpdate.Id);
                }
            }
        }

        [Receive(nameof(Incoming.StuffDataUpdate))]
        protected void HandleStuffDataUpdate(IReadOnlyPacket packet)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("[{method}] Not in room.", nameof(HandleStuffDataUpdate));
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("[{method}] Current room is null.", nameof(HandleStuffDataUpdate));
                return;
            }

            long id = packet.Protocol switch
            {
                ClientType.Flash => long.Parse(packet.ReadString()),
                ClientType.Unity => packet.ReadLong(),
                _ => throw new InvalidOperationException("Unknown protocol.")
            };

            if (!_currentRoom.FloorItems.TryGetValue(id, out FloorItem? item))
            {
                _logger.LogError("[{method}] Unable to find floor item {id} to update.", nameof(HandleStuffDataUpdate), id);
                return;
            }

            StuffData previousData = item.Data;
            item.Data = StuffData.Parse(packet);

            OnFloorItemDataUpdated(item, previousData);
        }

        [Receive(nameof(Incoming.MultipleStuffDataUpdate))]
        protected void HandleItemsDataUpdate(IReadOnlyPacket packet)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("[{method}] Not in room.", nameof(HandleItemsDataUpdate));
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("[{method}] Current room is null.", nameof(HandleItemsDataUpdate));
                return;
            }

            short n = packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
            {
                long itemId = packet.ReadLegacyLong();
                var data = StuffData.Parse(packet);
                if (!_currentRoom.FloorItems.TryGetValue(itemId, out FloorItem? item))
                {
                    _logger.LogError("[{method}] Failed to find floor item {id} to update.", itemId, nameof(HandleItemsDataUpdate));
                    continue;
                }

                StuffData previousData = item.Data;
                item.Data = data;

                OnFloorItemDataUpdated(item, previousData);
            }
        }
        #endregion

        #region - Wall item packet handlers -
        [Receive(nameof(Incoming.Items))]
        protected void HandleItems(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom)
            {
                _logger.LogDebug("Not loading room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            List<WallItem> newItems = new List<WallItem>();

            WallItem[] items = WallItem.ParseAll(packet);
            foreach (var item in items)
            {
                if (_currentRoom.WallItems.TryAdd(item.Id, item))
                {
                    newItems.Add(item);
                }
                else
                {
                    _logger.LogError("Failed to add wall item {itemId}.", item.Id);
                }
            }

            if (newItems.Count > 0)
            {
                OnWallItemsLoaded(newItems);
            }
        }

        [Receive(nameof(Incoming.AddItem))]
        protected void HandleAddItem(IReadOnlyPacket packet)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            var item = WallItem.Parse(packet);
            if (_currentRoom.WallItems.TryAdd(item.Id, item))
            {
                OnWallItemAdded(item);
            }
            else
            {
                _logger.LogError("Failed to add wall item {itemId}.", item.Id);
            }
        }

        [Receive(nameof(Incoming.RemoveItem))]
        protected void HandleRemoveItem(IReadOnlyPacket packet)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            long id = packet.Protocol switch
            {
                ClientType.Flash => long.Parse(packet.ReadString()),
                ClientType.Unity => packet.ReadLong(),
                _ => throw new InvalidOperationException("Unknown protocol")
            };
            // long pickerId

            if (_currentRoom.WallItems.TryRemove(id, out WallItem? item))
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
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            WallItem updatedItem = WallItem.Parse(packet);
            WallItem? previousItem = null;

            updatedItem = _currentRoom.WallItems.AddOrUpdate(
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

            if (previousItem is null)
            {
                _logger.LogError("Failed to find previous wall item {itemId} to update", updatedItem.Id);
            }
            else
            {
                OnWallItemUpdated(previousItem, updatedItem);
            }

            /*if (wallItems.TryGetValue(updatedItem.Id, out WallItem previousItem))
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
            }*/
        }
        #endregion

        #region - Entity packet handlers -
        [InterceptIn(nameof(Incoming.RoomUsers))]
        private void HandleUsersInRoom(InterceptArgs e)
        {
            if (!IsLoadingRoom && !IsInRoom)
            {
                _logger.LogDebug("Not loading room / not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            List<Entity> newEntities = new List<Entity>();

            foreach (Entity entity in Entity.ParseAll(e.Packet))
            {
                if (_currentRoom.Entities.TryAdd(entity.Index, entity))
                {
                    newEntities.Add(entity);
                    OnEntityAdded(entity);
                }
                else
                {
                    _logger.LogError("Failed to add entity {index} {name} (id:{id})", entity.Index, entity.Name, entity.Id);
                }
            }

            if (newEntities.Count > 0)
            {
                OnEntitiesAdded(newEntities);
            }
        }

        [InterceptIn(nameof(Incoming.UserLoggedOut))]
        private void HandleUserLoggedOut(InterceptArgs e)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            int index = e.Client switch
            {
                ClientType.Flash => int.Parse(e.Packet.ReadString()),
                ClientType.Unity => e.Packet.ReadInt(),
                _ => throw new InvalidOperationException("Unknown protocol")
            };

            if (_currentRoom.Entities.TryRemove(index, out Entity? entity))
            {
                OnEntityRemoved(entity);
            }
            else
            {
                _logger.LogError("Failed to remove entity with index {index}", index);
            }
        }

        [InterceptIn(nameof(Incoming.Status))]
        private void HandleStatus(InterceptArgs e)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            var updatedEntities = new List<IEntity>();

            short n = e.Packet.ReadLegacyShort();
            for (int i = 0; i < n; i++)
            {
                EntityStatusUpdate update = EntityStatusUpdate.Parse(e.Packet);
                if (!_currentRoom.Entities.TryGetValue(update.Index, out Entity? entity))
                {
                    _logger.LogError("Failed to find entity with index {index} to update", update.Index);
                    continue;
                }

                entity.Update(update);
                updatedEntities.Add(entity);

                OnEntityUpdated(entity);
            }

            OnEntitiesUpdated(updatedEntities);
        }

        [InterceptIn(nameof(Incoming.QueueMoveUpdate))]
        private void HandleQueueMoveUpdate(InterceptArgs e)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            RollerUpdate rollerUpdate = RollerUpdate.Parse(e.Packet);

            if (rollerUpdate.Type == RollerUpdateType.MovingEntity ||
                rollerUpdate.Type == RollerUpdateType.StationaryEntity)
            {
                if (_currentRoom.Entities.TryGetValue(rollerUpdate.EntityIndex, out Entity? entity))
                {
                    var previousTile = entity.Location;
                    entity.Location = new Tile(rollerUpdate.TargetX, rollerUpdate.TargetY, rollerUpdate.EntityTargetZ);

                    OnEntitySlide(entity, previousTile);
                }
                else
                {
                    _logger.LogError("Failed to find entity with index {index} to update.", rollerUpdate.EntityIndex);
                }
            }
        }

        [InterceptIn(nameof(Incoming.UpdateAvatar))]
        private void HandleUpdateAvatar(InterceptArgs e)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room."); 
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            int index = e.Packet.ReadInt();
            if (_currentRoom.Entities.TryGetValue(index, out Entity? entity) &&
                entity is RoomUser user)
            {
                string previousFigure = user.Figure;
                Gender previousGender = user.Gender;
                string previousMotto = user.Motto;
                int previousAchievementScore = user.AchievementScore;

                user.Figure = e.Packet.ReadString();
                user.Gender = H.ToGender(e.Packet.ReadString());
                user.Motto = e.Packet.ReadString();
                user.AchievementScore = e.Packet.ReadInt();

                OnUserDataUpdated(user,
                    previousFigure, previousGender,
                    previousMotto, previousAchievementScore
                );
            }
            else
            {
                _logger.LogError("Failed to find entity with index {index} to update.", index);
            }
        }

        [InterceptIn(nameof(Incoming.UserNameChanged))] // @Check
        private void HandleUserNameChanged(InterceptArgs e)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            long id = e.Packet.ReadLegacyLong();
            int index = e.Packet.ReadInt();
            string newName = e.Packet.ReadString();

            if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
            {
                string previousName = entity.Name;
                entity.Name = newName;
                OnEntityNameChanged(entity, previousName);
            }
            else
            {
                _logger.LogError("Failed to find entity with index {index} to update.", index);
            }
        }

        [InterceptIn(nameof(Incoming.RoomAvatarSleeping))]
        private void HandleRoomAvatarSleeping(InterceptArgs e)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            int index = e.Packet.ReadInt();
            if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
            {
                bool wasIdle = entity.IsIdle;
                entity.IsIdle = e.Packet.ReadBool();
                OnEntityIdle(entity, wasIdle);
            }
            else
            {
                _logger.LogError("Failed to find entity with index {index} to update.", index);
            }
        }

        [InterceptIn(nameof(Incoming.RoomDance))]
        private void HandleRoomDance(InterceptArgs e)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            int index = e.Packet.ReadInt();
            if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
            {
                int previousDance = entity.Dance;
                entity.Dance = e.Packet.ReadInt();
                OnEntityDance(entity, previousDance);
            }
            else
            {
                _logger.LogError("Failed to find entity with index {index} to update.", index);
                return;
            }
        }

        [InterceptIn(nameof(Incoming.RoomExpression))]
        private void HandleRoomExpression(InterceptArgs e)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            int index = e.Packet.ReadInt();
            if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
            {
                OnEntityExpression(entity, (Expressions)e.Packet.ReadInt());
            }
            else
            {
                _logger.LogError("Failed to find entity with index {index} to update.", index);
            }
        }

        [InterceptIn(nameof(Incoming.RoomCarryObject))]
        private void HandleRoomCarryObject(InterceptArgs e)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            int index = e.Packet.ReadInt();
            if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
            {
                int previousItem = entity.HandItem;
                entity.HandItem = e.Packet.ReadInt();
                OnEntityHandItem(entity, previousItem);
            }
            else
            {
                _logger.LogError("Failed to find entity with index {index} to update.", index);
            }
        }

        [InterceptIn(nameof(Incoming.RoomAvatarEffect))]
        private void HandleRoomAvatarEffect(InterceptArgs e)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            int index = e.Packet.ReadInt();
            // + int delay
            if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
            {
                int previousEffect = entity.Effect;
                entity.Effect = e.Packet.ReadInt();
                OnEntityEffect(entity, previousEffect);
            }
            else
            {
                _logger.LogError("Failed to find entity with index {index} to update.", index);
            }
        }

        [InterceptIn(nameof(Incoming.UserTypingStatusChange))]
        private void HandleUserTypingStatusChange(InterceptArgs e)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            int index = e.Packet.ReadInt();
            if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
            {
                bool wasTyping = entity.IsTyping;
                entity.IsTyping = e.Packet.ReadInt() != 0;
                OnEntityTyping(entity, wasTyping);
            }
            else
            {
                _logger.LogError("Failed to find entity with index {index} to update.", index);
            }
        }

        [InterceptIn(nameof(Incoming.Whisper), nameof(Incoming.Chat), nameof(Incoming.Shout))]
        private void HandleChat(InterceptArgs e)
        {
            if (!IsInRoom)
            {
                _logger.LogDebug("Not in room.");
                return;
            }

            if (_currentRoom is null)
            {
                _logger.LogWarning("Current room is null.");
                return;
            }

            ChatType chatType;

            if (e.Packet.Header == In.Whisper) chatType = ChatType.Whisper;
            else if (e.Packet.Header == In.Chat) chatType = ChatType.Talk;
            else if (e.Packet.Header == In.Shout) chatType = ChatType.Shout;
            else
            {
                _logger.LogError("Failed to detect chat type from header {header} !", e.Packet.Header);
                return;
            }

            int index = e.Packet.ReadInt();
            if (!_currentRoom.Entities.TryGetValue(index, out Entity? entity))
            {
                _logger.LogError("Failed to find entity with index {index}.", index);
                return;
            }

            string message = e.Packet.ReadString();
            int expression = e.Packet.ReadInt();
            int bubbleStyle = e.Packet.ReadInt();

            // string? int

            EntityChatEventArgs chatEventArgs = new(entity, chatType, message, bubbleStyle);
            OnEntityChat(chatEventArgs);

            if (chatEventArgs.IsBlocked || entity.IsHidden) e.Block();
        }
        #endregion

        #region - Furni interaction -
        public void Place(long itemId, int x, int y, int direction)
            => SendAsync(Out.PlaceRoomItem, itemId, x, y, direction);
        public void Place(long itemId, (int X, int Y) location, int direction)
            => SendAsync(Out.PlaceRoomItem, itemId, location.X, location.Y, direction);
        public void Place(IInventoryItem item, int x, int y, int direction)
            => Place(item.Id, x, y, direction);
        public void Place(IInventoryItem item, (int X, int Y) location, int direction)
            => Place(item.Id, location, direction);

        public void Place(long itemId, WallLocation location)
            => SendAsync(Out.PlaceWallItem, itemId, location.WallX, location.WallY, location.X, location.Y);
        public void Place(IInventoryItem item, WallLocation location)
            => Place(item.Id, location);

        public void Move(long floorItemId, int x, int y, int direction)
            => SendAsync(Out.MoveRoomItem, floorItemId, x, y, direction);
        public void Move(long floorItemId, (int X, int Y) location, int direction)
            => SendAsync(Out.MoveRoomItem, floorItemId, location.X, location.Y, direction);
        public void Move(IFloorItem item, int x, int y, int direction)
            => Move(item.Id, x, y, direction);
        public void Move(IFloorItem item, (int X, int Y) location, int direction)
            => Move(item.Id, location.X, location.Y, direction);

        public void Move(long wallItemId, WallLocation location) => SendAsync(
            Out.MoveWallItem, wallItemId,
            location.WallX, location.WallY,
            location.X, location.Y,
            location.Orientation.Value.ToString()
        );
        public void Move(IWallItem item, WallLocation location)
            => Move(item.Id, location);

        public void Pickup(IFurni item) => Pickup(item.Type, item.Id);
        public void Pickup(ItemType type, long id)
        {
            if (type == ItemType.Floor)
                SendAsync(Out.PickItemUpFromRoom, 2, id);
            else if (type == ItemType.Wall)
                SendAsync(Out.PickItemUpFromRoom, 1, id);
        }

        public void UpdateStackTile(IFloorItem stackTile, float height) => UpdateStackTile(stackTile.Id, height);
        public void UpdateStackTile(long stackTileId, float height)
            => SendAsync(Out.StackingHelperSetCaretHeight, stackTileId, (int)Math.Round(height * 100.0));
        #endregion
    }
}
