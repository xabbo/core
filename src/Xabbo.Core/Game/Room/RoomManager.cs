using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Interceptor;
using Xabbo.Messages.Flash;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

/// <summary>
/// Manages information about the current room, the user's permissions in the room, its furni, entities and chat.
/// </summary>
[Intercepts]
public sealed partial class RoomManager : GameStateManager
{
    private readonly ILogger _logger;
    private readonly Dictionary<long, RoomData> _roomDataCache = new();

    private Room? _currentRoom;
    private RoomData? _currentRoomData;

    private bool _gotHeightMap, _gotUsers, _gotObjects, _gotItems;

    /// <summary>
    /// Gets the ID of the current room. The <see cref="Room"/> may not be available
    /// even when the current room ID is set (e.g. when in the queue).
    /// </summary>
    private long _currentRoomId = -1;
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
    public event Action? EnteredQueue;
    private void OnEnteredQueue()
    {
        _logger.LogTrace("Entered queue. (pos:{queuePosition})", QueuePosition);
        EnteredQueue?.Invoke();
    }

    /// <summary>
    /// Invoked when the user's position in the queue is updated.
    /// </summary>
    public event Action? QueuePositionUpdated;
    private void OnQueuePositionUpdated()
    {
        _logger.LogTrace("Queue position updated. (pos:{queuePosition})", QueuePosition);
        QueuePositionUpdated?.Invoke();
    }

    /// <summary>
    /// Invoked when the user enters a room and begins loading the room state.
    /// </summary>
    public event Action? Entering;
    private void OnEnteringRoom(long roomId)
    {
        _logger.LogTrace("Entering room. (id:{roomId})", roomId);
        Entering?.Invoke();
    }

    /// <summary>
    /// Invoked after the user has entered the room and the room state is fully loaded.
    /// </summary>
    public event Action<RoomEventArgs>? Entered;
    private void OnEnteredRoom(IRoom room)
    {
        _logger.LogTrace("Entered room. (id:{roomId})", room.Id);
        Entered?.Invoke(new RoomEventArgs(room));
    }

    /// <summary>Invoked when the room data is updated.</summary>
    public event Action<RoomDataEventArgs>? RoomDataUpdated;
    private void OnRoomDataUpdated(RoomData roomData)
    {
        _logger.LogTrace("Room data updated. (name:{name})", roomData.Name);
        RoomDataUpdated?.Invoke(new RoomDataEventArgs(roomData));
    }

    /// <summary>
    /// Invoked when the user leaves a room.
    /// </summary>
    public event Action? Left;
    private void OnLeftRoom()
    {
        _logger.LogTrace("Left room.");
        Left?.Invoke();
    }

    /// <summary>
    /// Invoked when the user is kicked from the room.
    /// The user still remains in the room at this point until their avatar leaves through the door.
    /// </summary>
    public event Action? Kicked;
    private void OnKicked()
    {
        _logger.LogTrace("Kicked from room.");
        Kicked?.Invoke();
    }

    /// <summary>
    /// Invoked when the user's rights to the room are updated.
    /// </summary>
    public event Action? RightsUpdated;
    private void OnRightsUpdated()
    {
        _logger.LogTrace("Rights updated.");
        RightsUpdated?.Invoke();
    }
    #endregion

    #region - Furni events -
    /// <summary>
    /// Invoked when the floor items are loaded.
    /// This may happen multiple times depending on
    /// how many items are in the room.
    /// </summary>
    public event Action<FloorItemsEventArgs>? FloorItemsLoaded;
    private void OnFloorItemsLoaded(IEnumerable<IFloorItem> items)
    {
        _logger.LogTrace("Floor items loaded. ({n})", items.Count());
        FloorItemsLoaded?.Invoke(new FloorItemsEventArgs(items));
    }

    /// <summary>
    /// Invoked when a floor item is added to the room.
    /// </summary>
    public event Action<FloorItemEventArgs>? FloorItemAdded;
    private void OnFloorItemAdded(IFloorItem item)
    {
        _logger.LogTrace("Floor item added. (id:{id})", item.Id);
        FloorItemAdded?.Invoke(new FloorItemEventArgs(item));
    }

    /// <summary>
    /// Invoked when a floor item is updated.
    /// </summary>
    public event Action<FloorItemUpdatedEventArgs>? FloorItemUpdated;
    private void OnFloorItemUpdated(IFloorItem previousItem, IFloorItem updatedItem)
    {
        _logger.LogTrace("Floor item updated. (id:{id})", updatedItem.Id);
        FloorItemUpdated?.Invoke(new FloorItemUpdatedEventArgs(previousItem, updatedItem));
    }

    /// <summary>
    /// Invoked when a floor item's data is updated.
    /// </summary>
    public event Action<FloorItemDataUpdatedEventArgs>? FloorItemDataUpdated;
    private void OnFloorItemDataUpdated(IFloorItem item, IItemData previousData)
    {
        _logger.LogTrace("Floor item data updated. (id:{id})", item.Id);
        FloorItemDataUpdated?.Invoke(new FloorItemDataUpdatedEventArgs(item, previousData));
    }

    public event Action<DiceUpdatedEventArgs>? DiceUpdated;
    private void OnDiceUpdated(IFloorItem item, int previousValue, int currentValue)
    {
        _logger.LogTrace("Dice value updated. (id:{id})", item.Id);
        DiceUpdated?.Invoke(new DiceUpdatedEventArgs(item, previousValue, currentValue));
    }

    /// <summary>
    /// Invoked when a floor item slides due to a roller or wired update.
    /// </summary>
    public event Action<FloorItemSlideEventArgs>? FloorItemSlide;
    private void OnFloorItemSlide(IFloorItem item, Tile previousTile, long rollerId)
    {
        _logger.LogTrace(
            "Floor item slide. (id:{id}, rollerId:{rollerId}, {from} -> {to})",
            item.Id, rollerId, previousTile, item.Location
        );
        FloorItemSlide?.Invoke(new FloorItemSlideEventArgs(item, previousTile, rollerId));
    }

    /// <summary>
    /// Invoked when users or furni are moved by wired.
    /// </summary>
    public event Action<WiredMovementsEventArgs>? WiredMovements;
    private void OnWiredMovements(IEnumerable<WiredMovement> movements)
    {
        WiredMovements?.Invoke(new WiredMovementsEventArgs(movements));
    }

    /// <summary>
    /// Invoked when a floor item is removed from the room.
    /// </summary>
    public event Action<FloorItemEventArgs>? FloorItemRemoved;
    private void OnFloorItemRemoved(IFloorItem item)
    {
        _logger.LogTrace("Floor item removed. (id:{id})", item.Id);
        FloorItemRemoved?.Invoke(new FloorItemEventArgs(item));
    }

    /// <summary>
    /// Invoked when the wall items are loaded.
    /// This may happen multiple times depending on
    /// how many items are in the room.
    /// </summary>
    public event Action<WallItemsEventArgs>? WallItemsLoaded;
    private void OnWallItemsLoaded(IEnumerable<IWallItem> items)
    {
        _logger.LogTrace("Wall items loaded. ({n})", items.Count());
        WallItemsLoaded?.Invoke(new WallItemsEventArgs(items));
    }

    /// <summary>
    /// Invoked when a wall item is added to the room.
    /// </summary>
    public event Action<WallItemEventArgs>? WallItemAdded;
    private void OnWallItemAdded(IWallItem item)
    {
        _logger.LogTrace("Wall item added. (id:{id})", item.Id);
        WallItemAdded?.Invoke(new WallItemEventArgs(item));
    }

    /// <summary>
    /// Invoked when a wall item is udpated.
    /// </summary>
    public event Action<WallItemUpdatedEventArgs>? WallItemUpdated;
    private void OnWallItemUpdated(IWallItem previousItem, IWallItem updatedItem)
    {
        _logger.LogTrace("Wall item updated. (id:{id})", updatedItem.Id);
        WallItemUpdated?.Invoke(new WallItemUpdatedEventArgs(previousItem, updatedItem));
    }

    /// <summary>
    /// Invoked when a wall item is removed from the room.
    /// </summary>
    public event Action<WallItemEventArgs>? WallItemRemoved;
    private void OnWallItemRemoved(IWallItem item)
    {
        _logger.LogTrace("Wall item removed. (id:{id})", item.Id);
        WallItemRemoved?.Invoke(new WallItemEventArgs(item));
    }

    /// <summary>
    /// Invoked when a furni's visibility is toggled using <see cref="HideFurni(IFurni)"/> or <see cref="ShowFurni(IFurni)"/>.
    /// </summary>
    public event Action<FurniEventArgs>? FurniVisibilityToggled;
    private void OnFurniVisibilityToggled(IFurni furni)
    {
        FurniVisibilityToggled?.Invoke(new FurniEventArgs(furni));
    }
    #endregion

    #region - Entity events -
    /// <summary>
    /// Invoked when an entity has been added to the room.
    /// </summary>
    public event Action<EntityEventArgs>? EntityAdded;
    private void OnEntityAdded(IEntity entity)
    {
        EntityAdded?.Invoke(new EntityEventArgs(entity));
    }

    /// <summary>
    /// Invoked when entities have been added to the room.
    /// </summary>
    public event Action<EntitiesEventArgs>? EntitiesAdded;
    private void OnEntitiesAdded(IEnumerable<IEntity> entities)
    {
        _logger.LogTrace("Entities added. ({n})", entities.Count());
        EntitiesAdded?.Invoke(new EntitiesEventArgs(entities));
    }

    /// <summary>
    /// Invoked when an entity in the room is updated.
    /// </summary>
    public event Action<EntityEventArgs>? EntityUpdated;
    private void OnEntityUpdated(IEntity entity)
    {
        EntityUpdated?.Invoke(new EntityEventArgs(entity));
    }

    /// <summary>
    /// Invoked when entities in the room are updated.
    /// </summary>
    public event Action<EntitiesEventArgs>? EntitiesUpdated;
    private void OnEntitiesUpdated(IEnumerable<IEntity> entities)
    {
        _logger.LogTrace("Entities updated. ({n})", entities.Count());
        EntitiesUpdated?.Invoke(new EntitiesEventArgs(entities));
    }

    /// <summary>
    /// Invoked when an entity slides along a roller.
    /// </summary>
    public event Action<EntitySlideEventArgs>? EntitySlide;
    private void OnEntitySlide(IEntity entity, Tile previousTile)
    {
        _logger.LogTrace(
            "Entity slide. ({entityName} [{entityId}:{entityIndex}], {from} -> {to})",
            entity.Name, entity.Id, entity.Index, previousTile, entity.Location
        );
        EntitySlide?.Invoke(new EntitySlideEventArgs(entity, previousTile));
    }

    /// <summary>
    /// Invoked when an entity's figure, gender, motto or achievement score is updated.
    /// </summary>
    public event Action<EntityDataUpdatedEventArgs>? EntityDataUpdated;
    private void OnEntityDataUpdated(IEntity entity,
        string previousFigure, Gender previousGender,
        string previousMotto, int previousAchievementScore)
    {
        _logger.LogTrace(
            "Entity data updated. ({name} [{id}:{index}])",
            entity.Name, entity.Id, entity.Index
        );
        EntityDataUpdated?.Invoke(new EntityDataUpdatedEventArgs(
            entity, previousFigure, previousGender,
            previousMotto, previousAchievementScore
        ));
    }

    /// <summary>
    /// Invoked when an entity's name changes.
    /// </summary>
    public event Action<EntityNameChangedEventArgs>? EntityNameChanged;
    private void OnEntityNameChanged(IEntity entity, string previousName)
    {
        _logger.LogTrace(
            "Entity name changed. ({previousName} -> {entityName} [{entityId}:{entityIndex}])",
            previousName, entity.Name, entity.Id, entity.Index
        );
        EntityNameChanged?.Invoke(new EntityNameChangedEventArgs(entity, previousName));
    }

    /// <summary>
    /// Invoked when an entity's idle status updates.
    /// </summary>
    public event Action<EntityIdleEventArgs>? EntityIdle;
    private void OnEntityIdle(IEntity entity, bool wasIdle)
    {
        _logger.LogTrace(
            "Entity idle. ({entityName} [{entityId}:{entityIndex}], {wasIdle} -> {isIdle})",
            entity.Name, entity.Id, entity.Index, wasIdle, entity.IsIdle
        );
        EntityIdle?.Invoke(new EntityIdleEventArgs(entity, wasIdle));
    }

    /// <summary>
    /// Invoked when an entity's dance updates.
    /// </summary>
    public event Action<EntityDanceEventArgs>? EntityDance;
    private void OnEntityDance(IEntity entity, int previousDance)
    {
        _logger.LogTrace(
            "Entity dance. ({entityName} [{entityId}:{entityIndex}], {previousDance} -> {dance})",
            entity.Name, entity.Id, entity.Index, previousDance, entity.Dance
        );
        EntityDance?.Invoke(new EntityDanceEventArgs(entity, previousDance));
    }

    /// <summary>
    /// Invoked when an entity's hand item updates.
    /// </summary>
    public event Action<EntityHandItemEventArgs>? EntityHandItem;
    private void OnEntityHandItem(IEntity entity, int previousItem)
    {
        _logger.LogTrace(
            "Entity hand item. ({entityName} [{entityId}:{entityIndex}], {previousItemId} -> {itemId})",
            entity.Name, entity.Id, entity.Index, previousItem, entity.HandItem
        );
        EntityHandItem?.Invoke(new EntityHandItemEventArgs(entity, previousItem));
    }

    /// <summary>
    /// Invoked when an entity's effect updates.
    /// </summary>
    public event Action<EntityEffectEventArgs>? EntityEffect;
    private void OnEntityEffect(IEntity entity, int previousEffect)
    {
        _logger.LogTrace(
            "Entity effect. ({entityName} [{entityId}:{entityIndex}], {previousEffect} -> {effect})",
            entity.Name, entity.Id, entity.Index, previousEffect, entity.Effect
        );
        EntityEffect?.Invoke(new EntityEffectEventArgs(entity, previousEffect));
    }

    /// <summary>
    /// Invoked when an entity performs an action.
    /// </summary>
    public event Action<EntityActionEventArgs>? EntityAction;
    private void OnEntityAction(IEntity entity, Actions action)
    {
        _logger.LogTrace(
            "Entity action. ({entityName} [{entityId}:{entityIndex}], action:{action})",
            entity.Name, entity.Id, entity.Index, action
        );
        EntityAction?.Invoke(new EntityActionEventArgs(entity, action));
    }

    /// <summary>
    /// Invoked when an entity's typing status updates.
    /// </summary>
    public event Action<EntityTypingEventArgs>? EntityTyping;
    private void OnEntityTyping(IEntity entity, bool wasTyping)
    {
        _logger.LogTrace(
            "Entity typing. ({entityName} [{entityId}:{entityIndex}], {wasTyping} -> {isTyping})",
            entity.Name, entity.Id, entity.Index, wasTyping, entity.IsTyping
        );
        EntityTyping?.Invoke(new EntityTypingEventArgs(entity, wasTyping));
    }

    /// <summary>
    /// Invoked when an entity is removed from the room.
    /// </summary>
    public event Action<EntityEventArgs>? EntityRemoved;
    private void OnEntityRemoved(IEntity entity)
    {
        _logger.LogTrace(
            "Entity removed. ({entityName} [{entityId}:{entityIndex}])",
            entity.Name, entity.Id, entity.Index
        );
        EntityRemoved?.Invoke(new EntityEventArgs(entity));
    }

    /// <summary>
    /// Invoked when an entity in the room talks.
    /// </summary>
    public event Action<EntityChatEventArgs>? EntityChat;
    private void OnEntityChat(EntityChatEventArgs e)
    {
        _logger.LogTrace(
            "{type}:{bubble} {entity} {message}",
            e.ChatType, e.BubbleStyle, e.Entity, e.Message
        );
        EntityChat?.Invoke(e);
    }
    #endregion

    /// <summary>
    /// Retrieves the room data from the cache if it is available.
    /// </summary>
    public bool TryGetRoomData(long roomId, [NotNullWhen(true)] out RoomData? data) => _roomDataCache.TryGetValue(roomId, out data);

    public RoomManager(ILogger<RoomManager> logger, IInterceptor interceptor)
        : base(interceptor)
    {
        _logger = logger;
    }

    public RoomManager(IInterceptor interceptor)
        : base(interceptor)
    {
        _logger = NullLogger.Instance;
    }

    protected override void OnDisconnected() => ResetState();

    private void EnteringRoom(Id id)
    {
        CurrentRoomId = id;

        if (_roomDataCache.TryGetValue(id, out RoomData? data))
        {
            _currentRoomData = data;
            _logger.LogTrace("Loaded room data from cache.");
        }
        else
        {
            _logger.LogTrace("Failed to load room data from cache.");
        }

        _currentRoom = new Room(id, data!);
    }

    // Checks the load state and enters the room on Shockwave.
    private void EnterRoomOnceLoaded()
    {
        if (IsInRoom) return;

        if (_currentRoom is not null &&
            _gotHeightMap && _gotUsers && _gotObjects && _gotItems)
        {
            EnterRoom(_currentRoom);
        }
    }

    private void EnterRoom(Room room)
    {
        IsLoadingRoom = false;
        IsInRoom = true;
        Room = room;

        OnEnteredRoom(room);
    }

    private void ResetState()
    {
        _logger.LogTrace("Resetting room state");

        IsInQueue =
        IsLoadingRoom =
        IsInRoom = false;
        QueuePosition = 0;

        RightsLevel = 0;
        IsOwner = false;

        Room = _currentRoom = null;
        Data = _currentRoomData = null;

        CurrentRoomId = -1;

        _gotHeightMap = _gotUsers = _gotObjects = _gotItems = false;
    }

    private bool CheckPermission(ModerationPermissions? permissions)
    {
        if (!permissions.HasValue || !IsInRoom) return false;

        return permissions switch
        {
            ModerationPermissions.OwnerOnly => IsOwner,
            ModerationPermissions.GroupAdmins => RightsLevel >= 3,
            ModerationPermissions.RightsHolders or
            (ModerationPermissions.GroupAdmins | ModerationPermissions.RightsHolders)
                => RightsLevel > 0,
            ModerationPermissions.AllUsers => true,
            _ => false,
        };
    }

    private bool SetFurniHidden(ItemType type, Id id, bool hide)
    {
        if (_currentRoom is null) return false;

        Furni furni;

        if (type == ItemType.Floor)
        {
            if (!_currentRoom.FloorItems.TryGetValue(id, out FloorItem? item) || item.IsHidden == hide)
                return false;

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
            if (!_currentRoom.WallItems.TryGetValue(id, out WallItem? item) || item.IsHidden == hide)
                return false;

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
            return false;
        }

        if (hide)
        {
            if (furni.Type == ItemType.Floor)
            {
                if (Interceptor.Session.IsUnity)
                    Interceptor.Send(In.ObjectRemove, furni.Id, false, -1L, 0);
                else
                    Interceptor.Send(In.ObjectRemove, furni.Id.ToString(), false, -1, 0);
            }
            else if (furni.Type == ItemType.Wall)
            {
                if (Interceptor.Session.IsUnity)
                    Interceptor.Send(In.ItemRemove, furni.Id, -1L);
                else
                    Interceptor.Send(In.ItemRemove, furni.Id.ToString(), -1);
            }
        }
        else
        {
            Interceptor.Send(furni.IsFloorItem ? In.ObjectAdd : In.ItemAdd, furni);
        }

        OnFurniVisibilityToggled(furni);
        return true;
    }

    public bool ShowFurni(ItemType type, Id id) => SetFurniHidden(type, id, false);
    public bool HideFurni(ItemType type, Id id) => SetFurniHidden(type, id, true);

    public bool ShowFurni(IFurni furni) => SetFurniHidden(furni.Type, furni.Id, false);
    public bool HideFurni(IFurni furni) => SetFurniHidden(furni.Type, furni.Id, true);

    #region - Room e.Packet handlers -
    [InterceptIn(nameof(In.GetGuestRoomResult))]
    private void HandleGetGuestRoomResult(Intercept e)
    {
        var roomData = e.Packet.Parse<RoomData>();

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

    [InterceptIn(nameof(In.OpenConnection))]
    private void HandleOpenConnection(Intercept e)
    {
        if (IsInQueue || IsRingingDoorbell || IsLoadingRoom || IsInRoom)
        {
            ResetState();
            OnLeftRoom();
        }

        // Room ID is not available yet on Shockwave.
        if (Interceptor.Session.Is(ClientType.Shockwave))
            return;

        EnteringRoom(e.Packet.Read<Id>());
    }

    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.RoomQueueStatus))]
    private void HandleRoomQueueStatus(Intercept e)
    {
        long roomId = e.Packet.Read<Id>();
        if (roomId != _currentRoom?.Id) return;

        if (e.Packet.Read<short>() == 2 &&
            e.Packet.Read<string>() == "visitors" &&
            e.Packet.Read<int>() == 2 &&
            e.Packet.Read<short>() == 1 &&
            e.Packet.Read<string>() == "visitors")
        {
            bool enteredQueue = !IsInQueue;

            IsInQueue = true;
            QueuePosition = e.Packet.Read<int>() + 1;

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

    [InterceptIn(nameof(In.YouAreSpectator))]
    private void HandleYouAreSpectator(Intercept e)
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

        long roomId = e.Packet.Read<Id>();
        if (roomId != _currentRoom.Id)
        {
            _logger.LogError("Room ID mismatch.");
            return;
        }

        IsSpectating = true;
    }

    [InterceptIn(nameof(In.RoomReady))]
    private void HandleRoomReady(Intercept e)
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

        string model;
        Id roomId;

        if (Interceptor.Session.Is(ClientType.Shockwave))
        {
            string[] fields = e.Packet.Read<string>().Split();
            model = fields[0];
            roomId = int.Parse(fields[1]);
        }
        else
        {
            model = e.Packet.Read<string>();
            roomId = e.Packet.Read<Id>();
        }

        CurrentRoomId = roomId;

        if (_currentRoom is null ||
            _currentRoom.Id != roomId)
        {
            if (_roomDataCache.TryGetValue(roomId, out RoomData? data))
            {
                _currentRoomData = data;
                _logger.LogTrace("Loaded room data from cache.");
            }
            else
            {
                _logger.LogTrace("Failed to load room data from cache.");
            }

            _currentRoom = new Room(roomId, data!);
        }

        _currentRoom.Model = model;
        IsLoadingRoom = true;

        OnEnteringRoom(roomId);
    }

    [InterceptIn(nameof(In.RoomProperty))]
    private void HandleRoomProperty(Intercept e)
    {
        if (!IsLoadingRoom)
        {
            _logger.LogWarning($"[{nameof(HandleRoomProperty)}] Not loading room!");
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogTrace($"[{nameof(HandleRoomProperty)}] Current room is null.");
            return;
        }

        string key = e.Packet.Read<string>();
        string value = e.Packet.Read<string>();
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

    [InterceptIn(nameof(In.YouAreController))]
    private void HandleYouAreController(Intercept e)
    {
        if (_currentRoom is null)
        {
            _logger.LogTrace($"[{nameof(HandleYouAreController)}] Current room is null.");
            return;
        }

        if (!Interceptor.Session.Is(ClientType.Shockwave))
        {
            long roomId = e.Packet.Read<Id>();
            if (roomId != _currentRoom.Id)
            {
                _logger.LogWarning($"[{nameof(HandleYouAreController)}] Room ID mismatch!");
                return;
            }
            RightsLevel = e.Packet.Read<int>();
        }
        else
        {
            RightsLevel = 4;
        }

        OnRightsUpdated();
    }

    [InterceptIn(nameof(In.YouAreNotController))]
    private void HandleYouAreNotController(Intercept e)
    {
        if (_currentRoom is null)
        {
            _logger.LogTrace($"[{nameof(HandleYouAreNotController)}] Current room is null.");
            return;
        }

        long roomId = e.Packet.Read<Id>();
        if (roomId != _currentRoom.Id)
        {
            _logger.LogWarning($"[{nameof(HandleYouAreNotController)}] Room ID mismatch!");
            return;
        }

        RightsLevel = 0;
        OnRightsUpdated();
    }

    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.RoomEntryTile))]
    private void HandleRoomEntryTile(Intercept e)
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

        int x = e.Packet.Read<int>();
        int y = e.Packet.Read<int>();
        int dir = e.Packet.Read<int>();

        _currentRoom.DoorTile = new Tile(x, y, 0);
        _currentRoom.EntryDirection = dir;

        _logger.LogTrace("Received room entry tile. (x:{x}, y:{y}, dir: {dir})", x, y, dir);
    }

    // Shockwave does not have a furni heightmap.
    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn("f:"+nameof(In.HeightMap))]
    private void HandleHeightMap(Intercept e)
    {
        if (!IsLoadingRoom)
        {
            _logger.LogWarning($"[{nameof(HandleHeightMap)}] Not loading room!");
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogTrace($"[{nameof(HandleHeightMap)}] Current room is null.");
            return;
        }

        Heightmap heightmap = e.Packet.Parse<Heightmap>();
        _currentRoom.Heightmap = heightmap;

        _logger.LogTrace("Received stacking heightmap. (size:{width}x{length})", heightmap.Width, heightmap.Length);
    }

    [InterceptIn(nameof(In.HeightMapUpdate))]
    private void HandleHeightMapUpdate(Intercept e)
    {
        if (_currentRoom is null)
        {
            _logger.LogTrace($"[{nameof(HandleHeightMapUpdate)}] Current room is null.");
            return;
        }

        if (_currentRoom.Heightmap is null) return;

        int n = e.Packet.Read<byte>();
        for (int i = 0; i < n; i++)
        {
            int x = e.Packet.Read<byte>();
            int y = e.Packet.Read<byte>();
            _currentRoom.Heightmap[x, y].Update(e.Packet.Read<short>());
        }

        _logger.LogTrace("Received stacking heightmap diff. ({n} change(s))", n);
    }

    [InterceptIn(nameof(In.FloorHeightMap))]
    private void HandleFloorHeightmap(Intercept e)
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

        FloorPlan floorPlan = e.Packet.Parse<FloorPlan>();
        _currentRoom.FloorPlan = floorPlan;

        _logger.LogTrace("Received floor heightmap. (size:{width}x{length})", floorPlan.Width, floorPlan.Length);

        if (Interceptor.Session.Is(ClientType.Shockwave))
        {
            _gotHeightMap = true;
            EnterRoomOnceLoaded();
        }
    }

    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.RoomVisualizationSettings))]
    private void HandleRoomVisualizationSettings(Intercept e)
    {
        if (_currentRoom is null)
        {
            return;
        }

        _currentRoom.HideWalls = e.Packet.Read<bool>();
        _currentRoom.WallThickness = (Thickness)e.Packet.Read<int>();
        _currentRoom.FloorThickness = (Thickness)e.Packet.Read<int>();

        // TODO event
    }

    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.RoomChatSettings))]
    private void HandleRoomChatSettings(Intercept e)
    {
        if (_currentRoom is null)
        {
            return;
        }

        _currentRoom.Data.ChatSettings = e.Packet.Parse<ChatSettings>();

        OnRoomDataUpdated(_currentRoom.Data);
    }

    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.RoomEntryInfo))]
    private void HandleRoomEntryInfo(Intercept e)
    {
        if (!IsLoadingRoom)
        {
            _logger.LogWarning($"[{nameof(HandleRoomEntryInfo)}] Not loading room!");
            return;
        }

        long roomId = e.Packet.Read<Id>();
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

        IsOwner = e.Packet.Read<bool>();

        EnterRoom(_currentRoom);
    }

    [InterceptIn(nameof(In.CloseConnection))]
    private void HandleCloseConnection(Intercept e)
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

    // TODO: Check how this works on Shockwave
    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.ErrorReport))]
    private void HandleErrorReport(Intercept e)
    {
        if (!IsInRoom) return;

        GenericErrorCode errorCode = (GenericErrorCode)e.Packet.Read<int>();
        if (errorCode == GenericErrorCode.Kicked)
            OnKicked();
    }
    #endregion

    #region - Floor item handlers -
    [InterceptIn("f:"+nameof(In.Objects))]
    private void HandleActiveObjects(Intercept e)
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

        List<FloorItem> newItems = [];

        FloorItem[] items = e.Packet.ParseAll<FloorItem>();
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

        if (Interceptor.Session.Is(ClientType.Shockwave))
        {
            _gotObjects = true;
            EnterRoomOnceLoaded();
        }
    }

    [InterceptIn(nameof(In.ObjectAdd))]
    private void HandleObjectAdd(Intercept e)
    {
        if (!IsInRoom)
        {
            _logger.LogDebug($"[{nameof(HandleObjectAdd)}] Not in room.");
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogWarning($"[{nameof(HandleObjectAdd)}] Current room is null.");
            return;
        }

        FloorItem item = e.Packet.Parse<FloorItem>();

        if (_currentRoom.FloorItems.TryAdd(item.Id, item))
        {
            OnFloorItemAdded(item);
        }
        else
        {
            _logger.LogError($"[{nameof(HandleObjectAdd)}] Failed to add floor item {{itemId}}.", item.Id);
        }
    }

    [InterceptIn(nameof(In.ObjectRemove))]
    private void HandleObjectRemove(Intercept e)
    {
        /*
            long id
            bool isExpired
            long pickerId
            int delay
        */

        if (!IsInRoom)
        {
            _logger.LogDebug("[{method}] Not in room.", nameof(HandleObjectRemove));
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogWarning("[{method}] Current room is null.", nameof(HandleObjectRemove));
            return;
        }

        long id = e.Packet.Client switch
        {
            ClientType.Flash => long.Parse(e.Packet.Read<string>()),
            ClientType.Unity => e.Packet.Read<long>(),
            _ => throw new InvalidOperationException("Unknown protocol type")
        };

        if (_currentRoom.FloorItems.TryRemove(id, out FloorItem? item))
        {
            OnFloorItemRemoved(item);
        }
        else
        {
            _logger.LogError("[{method}] Failed to remove item {id} from the dictionary", nameof(HandleObjectRemove), id);
        }
    }

    [InterceptIn(nameof(In.ObjectUpdate))]
    private void HandleObjectUpdate(Intercept e)
    {
        if (!IsInRoom)
        {
            _logger.LogDebug("[{method}] Not in room.", nameof(HandleObjectUpdate));
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogWarning("[{method}] Current room is null.", nameof(HandleObjectUpdate));
            return;
        }

        var updatedItem = e.Packet.Parse<FloorItem>();

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
                _logger.LogError("[{method}] Failed to update floor item {itemId}", nameof(HandleObjectUpdate), updatedItem.Id);
            }
        }
        else
        {
            _logger.LogError("[{method}] Failed to find floor item {itemId} to update", nameof(HandleObjectUpdate), updatedItem.Id);
        }
    }

    [InterceptIn(nameof(In.SlideObjectBundle))]
    private void HandleSlideObjectBundleObject(Intercept e)
    {
        if (!IsInRoom)
        {
            _logger.LogDebug("[{method}] Not in room.", nameof(HandleSlideObjectBundleObject));
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogWarning("[{method}] Current room is null.", nameof(HandleSlideObjectBundleObject));
            return;
        }

        RollerUpdate rollerUpdate = e.Packet.Parse<RollerUpdate>();
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
                _logger.LogError("[{method}] Failed to find floor item {itemId} to update.", nameof(HandleSlideObjectBundleObject), objectUpdate.Id);
            }
        }
    }

    [InterceptIn(nameof(In.ObjectDataUpdate))]
    private void HandleObjectDataUpdate(Intercept e)
    {
        if (!IsInRoom)
        {
            _logger.LogDebug("[{method}] Not in room.", nameof(HandleObjectDataUpdate));
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogWarning("[{method}] Current room is null.", nameof(HandleObjectDataUpdate));
            return;
        }

        long id = e.Packet.Client switch
        {
            ClientType.Flash => long.Parse(e.Packet.Read<string>()),
            ClientType.Unity => e.Packet.Read<long>(),
            _ => throw new InvalidOperationException("Unknown protocol.")
        };

        if (!_currentRoom.FloorItems.TryGetValue(id, out FloorItem? item))
        {
            _logger.LogError("[{method}] Unable to find floor item {id} to update.", nameof(HandleObjectDataUpdate), id);
            return;
        }

        IItemData previousData = item.Data;
        item.Data = e.Packet.Parse<ItemData>();

        OnFloorItemDataUpdated(item, previousData);
    }

    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.ObjectsDataUpdate))]
    private void HandleObjectsDataUpdate(Intercept e)
    {
        if (!IsInRoom)
        {
            _logger.LogDebug("[{method}] Not in room.", nameof(HandleObjectsDataUpdate));
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogWarning("[{method}] Current room is null.", nameof(HandleObjectsDataUpdate));
            return;
        }

        int n = e.Packet.Read<Length>();
        for (int i = 0; i < n; i++)
        {
            long itemId = e.Packet.Read<Id>();
            ItemData data = e.Packet.Parse<ItemData>();
            if (!_currentRoom.FloorItems.TryGetValue(itemId, out FloorItem? item))
            {
                _logger.LogError("[{method}] Failed to find floor item {id} to update.", itemId, nameof(HandleObjectsDataUpdate));
                continue;
            }

            ItemData previousData = item.Data;
            item.Data = data;

            OnFloorItemDataUpdated(item, previousData);
        }
    }

    [InterceptIn(nameof(In.DiceValue))]
    private void HandleDiceValue(Intercept e)
    {
        if (!IsInRoom) return;

        if (_currentRoom is null) return;

        Id itemId;
        int diceValue = -1;

        if (Interceptor.Session.Is(ClientType.Shockwave))
        {
            string[] fields = e.Packet.Read<string>().Split();
            itemId = int.Parse(fields[0]);
            if (fields.Length >= 2)
                diceValue = int.Parse(fields[1]) % (int)itemId;
        }
        else
        {
            itemId = e.Packet.Read<Id>();
            diceValue = e.Packet.Read<int>();
        }

        if (!_currentRoom.FloorItems.TryGetValue(itemId, out FloorItem? item))
        {
            _logger.LogError("[{method}] Failed to find floor item {id} to update.", nameof(HandleDiceValue), itemId);
            return;
        }

        int previousValue = item.Data.State;
        item.Data.Value = $"{diceValue}";

        OnDiceUpdated(item, previousValue, diceValue);
    }
    #endregion

    #region - Wall item handlers -
    [InterceptIn(nameof(In.Items))]
    private void HandleItems(Intercept e)
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

        List<WallItem> newItems = [];

        WallItem[] items = e.Packet.ParseAll<WallItem>();
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

        if (Interceptor.Session.Is(ClientType.Shockwave))
        {
            _gotItems = true;
            EnterRoomOnceLoaded();
        }
    }

    [InterceptIn(nameof(In.ItemAdd))]
    private void HandleItemAdd(Intercept e)
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

        var item = e.Packet.Parse<WallItem>();
        if (_currentRoom.WallItems.TryAdd(item.Id, item))
        {
            OnWallItemAdded(item);
        }
        else
        {
            _logger.LogError("Failed to add wall item {itemId}.", item.Id);
        }
    }

    [InterceptIn(nameof(In.ItemRemove))]
    private void HandleItemRemove(Intercept e)
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

        long id = e.Packet.Client switch
        {
            ClientType.Flash => long.Parse(e.Packet.Read<string>()),
            ClientType.Unity => e.Packet.Read<long>(),
            _ => throw new InvalidOperationException("Unknown protocol")
        };
        // long pickerId

        if (_currentRoom.WallItems.TryRemove(id, out WallItem? item))
        {
            OnWallItemRemoved(item);
        }
        else
        {
            Debug.Log($"failed to remove item {id} from the dictionary");
        }
    }

    [InterceptIn(nameof(In.ItemUpdate))]
    private void HandleItemUpdate(Intercept e)
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

        WallItem updatedItem = e.Packet.Parse<WallItem>();
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

    #region - Entity handlers -
    [InterceptIn(nameof(In.Users))]
    private void HandleUsers(Intercept e)
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

        List<Entity> newEntities = [];

        foreach (Entity entity in e.Packet.ParseArray<Entity>())
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

        if (IsLoadingRoom && !_gotUsers && Interceptor.Session.Is(ClientType.Shockwave))
        {
            _gotUsers = true;
            EnterRoomOnceLoaded();
        }
    }

    [InterceptIn(nameof(In.UserRemove))]
    private void HandleUserRemove(Intercept e)
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

        int index = e.Packet.Client switch
        {
            ClientType.Flash => int.Parse(e.Packet.Read<string>()),
            ClientType.Unity => e.Packet.Read<int>(),
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

    [InterceptIn(nameof(In.UserUpdate))]
    private void HandlerUserUpdate(Intercept e)
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

        int n = e.Packet.Read<Length>();
        for (int i = 0; i < n; i++)
        {
            EntityStatusUpdate update = e.Packet.Parse<EntityStatusUpdate>();
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

    [InterceptIn(nameof(In.SlideObjectBundle))]
    private void HandleSlideObjectBundleEntity(Intercept e)
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

        RollerUpdate rollerUpdate = e.Packet.Parse<RollerUpdate>();

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

    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.WiredMovements))]
    private void HandleWiredMovements(Intercept e)
    {
        var room = _currentRoom;

        int n = e.Packet.Read<int>();
        var movements = new WiredMovement[n];
        for (int i = 0; i < n; i++)
        {
            var movement = movements[i] = e.Packet.Parse<WiredMovement>();
            if (room is null) continue;
            switch (movement) {
                case UserWiredMovement m:
                    if (room.Entities.TryGetValue(m.UserIndex, out Entity? entity))
                        entity.Location = m.Destination;
                    break;
                case FloorItemWiredMovement m:
                    if (room.FloorItems.TryGetValue(m.FurniId, out FloorItem? item))
                        item.Location = m.Destination;
                    break;
                case WallItemWiredMovement m:
                    if (room.WallItems.TryGetValue(m.ItemId, out WallItem? wallItem))
                        wallItem.Location = m.Destination;
                    break;
            }
        }

        OnWiredMovements(movements);
    }

    // TODO: check
    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.UserChange))]
    private void HandleUserChange(Intercept e)
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

        int index = e.Packet.Read<int>();
        if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
        {
            string previousFigure = entity.Figure;
            Gender previousGender;
            string previousMotto = entity.Motto;
            int previousAchievementScore = 0;

            string updatedFigure = e.Packet.Read<string>();
            Gender updatedGender = H.ToGender(e.Packet.Read<string>());
            string updatedMotto = e.Packet.Read<string>();
            int updatedAchievementScore = e.Packet.Read<int>();

            entity.Figure = updatedFigure;
            entity.Motto = updatedMotto;

            if (entity is RoomUser user)
            {
                previousGender = user.Gender;
                user.Gender = updatedGender;
                previousAchievementScore = user.AchievementScore;
                user.AchievementScore = updatedAchievementScore;
            }
            else if (entity is Bot bot)
            {
                previousGender = bot.Gender;
                bot.Gender = updatedGender;
                previousAchievementScore = updatedAchievementScore;
            }
            else
            {
                previousGender = updatedGender;
                previousAchievementScore = updatedAchievementScore;
            }

            OnEntityDataUpdated(entity,
                previousFigure, previousGender,
                previousMotto, previousAchievementScore
            );
        }
        else
        {
            _logger.LogError("Failed to find entity with index {index} to update.", index);
        }
    }

    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.UserNameChanged))]
    private void HandleUserNameChanged(Intercept e)
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

        long id = e.Packet.Read<Id>();
        int index = e.Packet.Read<int>();
        string newName = e.Packet.Read<string>();

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

    // Shockwave does not have an avatar idle state.
    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.Sleep))]
    private void HandleSleep(Intercept e)
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

        int index = e.Packet.Read<int>();
        if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
        {
            bool wasIdle = entity.IsIdle;
            entity.IsIdle = e.Packet.Read<bool>();
            OnEntityIdle(entity, wasIdle);
        }
        else
        {
            _logger.LogError("Failed to find entity with index {index} to update.", index);
        }
    }

    // Shockwave uses a field in the entity's status update.
    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.Dance))]
    private void HandleDance(Intercept e)
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

        int index = e.Packet.Read<int>();
        if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
        {
            int previousDance = entity.Dance;
            entity.Dance = e.Packet.Read<int>();
            OnEntityDance(entity, previousDance);
        }
        else
        {
            _logger.LogError("Failed to find entity with index {index} to update.", index);
            return;
        }
    }

    // Shockwave uses a field in the entity's status update.
    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.Expression))]
    private void HandleExpression(Intercept e)
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

        int index = e.Packet.Read<int>();
        if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
        {
            OnEntityAction(entity, (Actions)e.Packet.Read<int>());
        }
        else
        {
            _logger.LogError("Failed to find entity with index {index} to update.", index);
        }
    }

    // Shockwave uses a field in the entity's status update.
    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.CarryObject))]
    private void HandleCarryObject(Intercept e)
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

        int index = e.Packet.Read<int>();
        if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
        {
            int previousItem = entity.HandItem;
            entity.HandItem = e.Packet.Read<int>();
            OnEntityHandItem(entity, previousItem);
        }
        else
        {
            _logger.LogError("Failed to find entity with index {index} to update.", index);
        }
    }

    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.AvatarEffect))]
    private void HandleAvatarEffect(Intercept e)
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

        int index = e.Packet.Read<int>();
        // + int delay
        if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
        {
            int previousEffect = entity.Effect;
            entity.Effect = e.Packet.Read<int>();
            OnEntityEffect(entity, previousEffect);
        }
        else
        {
            _logger.LogError("Failed to find entity with index {index} to update.", index);
        }
    }

    [InterceptsOn(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.UserTyping))]
    private void HandleUserTyping(Intercept e)
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

        int index = e.Packet.Read<int>();
        if (_currentRoom.Entities.TryGetValue(index, out Entity? entity))
        {
            bool wasTyping = entity.IsTyping;
            entity.IsTyping = e.Packet.Read<int>() != 0;
            OnEntityTyping(entity, wasTyping);
        }
        else
        {
            _logger.LogError("Failed to find entity with index {index} to update.", index);
        }
    }

    [InterceptIn(nameof(In.Whisper), nameof(In.Chat), nameof(In.Shout))]
    private void HandleChat(Intercept e)
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

        if (e.Is(In.Whisper)) chatType = ChatType.Whisper;
        else if (e.Is(In.Chat)) chatType = ChatType.Talk;
        else if (e.Is(In.Shout)) chatType = ChatType.Shout;
        else return;

        int index = e.Packet.Read<int>();
        if (!_currentRoom.Entities.TryGetValue(index, out Entity? entity))
        {
            _logger.LogError("Failed to find entity with index {index}.", index);
            return;
        }

        string message = e.Packet.Read<string>();

        int expression = 0, bubbleStyle = 0;
        if (Interceptor.Session.Is(~ClientType.Shockwave))
        {
            expression = e.Packet.Read<int>();
            bubbleStyle = e.Packet.Read<int>();
        }

        // string? int

        EntityChatEventArgs chatEventArgs = new(entity, chatType, message, bubbleStyle);
        OnEntityChat(chatEventArgs);

        if (chatEventArgs.IsBlocked || entity.IsHidden) e.Block();
    }
    #endregion

    #region - Furni interaction -
    public void Place(long itemId, int x, int y, int direction)
        => Interceptor.Send(Out.PlaceObject, itemId, x, y, direction);
    public void Place(long itemId, (int X, int Y) location, int direction)
        => Interceptor.Send(Out.PlaceObject, itemId, location.X, location.Y, direction);
    public void Place(IInventoryItem item, int x, int y, int direction)
        => Place(item.Id, x, y, direction);
    public void Place(IInventoryItem item, (int X, int Y) location, int direction)
        => Place(item.Id, location, direction);

    public void Place(long itemId, WallLocation location)
        => Interceptor.Send(Out.PlaceObject, itemId, location.WX, location.WY, location.LX, location.LY);
    public void Place(IInventoryItem item, WallLocation location)
        => Place(item.Id, location);

    public void Move(long floorItemId, int x, int y, int direction)
        => Interceptor.Send(Out.MoveObject, floorItemId, x, y, direction);
    public void Move(long floorItemId, (int X, int Y) location, int direction)
        => Interceptor.Send(Out.MoveObject, floorItemId, location.X, location.Y, direction);
    public void Move(IFloorItem item, int x, int y, int direction)
        => Move(item.Id, x, y, direction);
    public void Move(IFloorItem item, (int X, int Y) location, int direction)
        => Move(item.Id, location.X, location.Y, direction);

    public void Move(long wallItemId, WallLocation location) => Interceptor.Send(
        Out.MoveWallItem, wallItemId,
        location.WX, location.WY,
        location.LX, location.LY,
        location.Orientation.Value.ToString()
    );
    public void Move(IWallItem item, WallLocation location)
        => Move(item.Id, location);

    public void Pickup(IFurni item) => Pickup(item.Type, item.Id);
    public void Pickup(ItemType type, Id id)
    {
        if (type == ItemType.Floor)
            Interceptor.Send(Out.PickupObject, 2, id);
        else if (type == ItemType.Wall)
            Interceptor.Send(Out.PickupObject, 1, id);
    }

    public void UpdateStackTile(IFloorItem stackTile, float height) => UpdateStackTile(stackTile.Id, height);
    public void UpdateStackTile(long stackTileId, float height)
        => Interceptor.Send(Out.SetCustomStackingHeight, stackTileId, (int)Math.Round(height * 100.0));

    #endregion
}
