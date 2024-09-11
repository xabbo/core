using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Interceptor;
using Xabbo.Messages.Flash;

using Xabbo.Core.Events;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Game;

/// <summary>
/// Manages information about the current room, the user's permissions in the room, its furni, avatars and chat.
/// </summary>
[Intercept]
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
    private void OnFloorItemSlide(IFloorItem item, Tile previousTile, Id rollerId)
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

    #region - Avatar events -
    /// <summary>
    /// Invoked when an avatar has been added to the room.
    /// </summary>
    public event Action<AvatarEventArgs>? AvatarAdded;
    private void OnAvatarAdded(IAvatar avatar)
    {
        AvatarAdded?.Invoke(new AvatarEventArgs(avatar));
    }

    /// <summary>
    /// Invoked when avatars have been added to the room.
    /// </summary>
    public event Action<AvatarsEventArgs>? AvatarsAdded;
    private void OnAvatarsAdded(IEnumerable<IAvatar> avatars)
    {
        _logger.LogTrace("Avatars added. ({n})", avatars.Count());
        AvatarsAdded?.Invoke(new AvatarsEventArgs(avatars));
    }

    /// <summary>
    /// Invoked when an avatar in the room is updated.
    /// </summary>
    public event Action<AvatarEventArgs>? AvatarUpdated;
    private void OnAvatarUpdated(IAvatar avatar)
    {
        AvatarUpdated?.Invoke(new AvatarEventArgs(avatar));
    }

    /// <summary>
    /// Invoked when avatars in the room are updated.
    /// </summary>
    public event Action<AvatarsEventArgs>? AvatarsUpdated;
    private void OnAvatarsUpdated(IEnumerable<IAvatar> avatars)
    {
        _logger.LogTrace("Avatars updated. ({n})", avatars.Count());
        AvatarsUpdated?.Invoke(new AvatarsEventArgs(avatars));
    }

    /// <summary>
    /// Invoked when an avatar slides along a roller.
    /// </summary>
    public event Action<AvatarSlideEventArgs>? AvatarSlide;
    private void OnAvatarSlide(IAvatar avatar, Tile previousTile)
    {
        _logger.LogTrace(
            "Avatar slide. ({avatarName} [{avatarId}:{avatarIndex}], {from} -> {to})",
            avatar.Name, avatar.Id, avatar.Index, previousTile, avatar.Location
        );
        AvatarSlide?.Invoke(new AvatarSlideEventArgs(avatar, previousTile));
    }

    /// <summary>
    /// Invoked when an avatar's figure, gender, motto or achievement score is updated.
    /// </summary>
    public event Action<AvatarDataUpdatedEventArgs>? AvatarDataUpdated;
    private void OnAvatarDataUpdated(IAvatar avatar,
        string previousFigure, Gender previousGender,
        string previousMotto, int previousAchievementScore)
    {
        _logger.LogTrace(
            "Avatar data updated. ({name} [{id}:{index}])",
            avatar.Name, avatar.Id, avatar.Index
        );
        AvatarDataUpdated?.Invoke(new AvatarDataUpdatedEventArgs(
            avatar, previousFigure, previousGender,
            previousMotto, previousAchievementScore
        ));
    }

    /// <summary>
    /// Invoked when an avatar's name changes.
    /// </summary>
    public event Action<AvatarNameChangedEventArgs>? AvatarNameChanged;
    private void OnAvatarNameChanged(IAvatar avatar, string previousName)
    {
        _logger.LogTrace(
            "Avatar name changed. ({previousName} -> {avatarName} [{avatarId}:{avatarIndex}])",
            previousName, avatar.Name, avatar.Id, avatar.Index
        );
        AvatarNameChanged?.Invoke(new AvatarNameChangedEventArgs(avatar, previousName));
    }

    /// <summary>
    /// Invoked when an avatar's idle status updates.
    /// </summary>
    public event Action<AvatarIdleEventArgs>? AvatarIdle;
    private void OnAvatarIdle(IAvatar avatar, bool wasIdle)
    {
        _logger.LogTrace(
            "Avatar idle. ({avatarName} [{avatarId}:{avatarIndex}], {wasIdle} -> {isIdle})",
            avatar.Name, avatar.Id, avatar.Index, wasIdle, avatar.IsIdle
        );
        AvatarIdle?.Invoke(new AvatarIdleEventArgs(avatar, wasIdle));
    }

    /// <summary>
    /// Invoked when an avatar's dance updates.
    /// </summary>
    public event Action<AvatarDanceEventArgs>? AvatarDance;
    private void OnAvatarDance(IAvatar avatar, Dances previousDance)
    {
        _logger.LogTrace(
            "Avatar dance. ({avatarName} [{avatarId}:{avatarIndex}], {previousDance} -> {dance})",
            avatar.Name, avatar.Id, avatar.Index, previousDance, avatar.Dance
        );
        AvatarDance?.Invoke(new AvatarDanceEventArgs(avatar, previousDance));
    }

    /// <summary>
    /// Invoked when an avatar's hand item updates.
    /// </summary>
    public event Action<AvatarHandItemEventArgs>? AvatarHandItem;
    private void OnAvatarHandItem(IAvatar avatar, int previousItem)
    {
        _logger.LogTrace(
            "Avatar hand item. ({avatarName} [{avatarId}:{avatarIndex}], {previousItemId} -> {itemId})",
            avatar.Name, avatar.Id, avatar.Index, previousItem, avatar.HandItem
        );
        AvatarHandItem?.Invoke(new AvatarHandItemEventArgs(avatar, previousItem));
    }

    /// <summary>
    /// Invoked when an avatar's effect updates.
    /// </summary>
    public event Action<AvatarEffectEventArgs>? AvatarEffect;
    private void OnAvatarEffect(IAvatar avatar, int previousEffect)
    {
        _logger.LogTrace(
            "Avatar effect. ({avatarName} [{avatarId}:{avatarIndex}], {previousEffect} -> {effect})",
            avatar.Name, avatar.Id, avatar.Index, previousEffect, avatar.Effect
        );
        AvatarEffect?.Invoke(new AvatarEffectEventArgs(avatar, previousEffect));
    }

    /// <summary>
    /// Invoked when an avatar performs an action.
    /// </summary>
    public event Action<AvatarActionEventArgs>? AvatarAction;
    private void OnAvatarAction(IAvatar avatar, Actions action)
    {
        _logger.LogTrace(
            "Avatar action. ({avatarName} [{avatarId}:{avatarIndex}], action:{action})",
            avatar.Name, avatar.Id, avatar.Index, action
        );
        AvatarAction?.Invoke(new AvatarActionEventArgs(avatar, action));
    }

    /// <summary>
    /// Invoked when an avatar's typing status updates.
    /// </summary>
    public event Action<AvatarTypingEventArgs>? AvatarTyping;
    private void OnAvatarTyping(IAvatar avatar, bool wasTyping)
    {
        _logger.LogTrace(
            "Avatar typing. ({avatarName} [{avatarId}:{avatarIndex}], {wasTyping} -> {isTyping})",
            avatar.Name, avatar.Id, avatar.Index, wasTyping, avatar.IsTyping
        );
        AvatarTyping?.Invoke(new AvatarTypingEventArgs(avatar, wasTyping));
    }

    /// <summary>
    /// Invoked when an avatar is removed from the room.
    /// </summary>
    public event Action<AvatarEventArgs>? AvatarRemoved;
    private void OnAvatarRemoved(IAvatar avatar)
    {
        _logger.LogTrace(
            "Avatar removed. ({avatarName} [{avatarId}:{avatarIndex}])",
            avatar.Name, avatar.Id, avatar.Index
        );
        AvatarRemoved?.Invoke(new AvatarEventArgs(avatar));
    }

    /// <summary>
    /// Invoked when an avatar in the room talks.
    /// </summary>
    public event Action<AvatarChatEventArgs>? AvatarChat;
    private void OnAvatarChat(AvatarChatEventArgs e)
    {
        _logger.LogTrace(
            "{type}:{bubble} {avatar} {message}",
            e.ChatType, e.BubbleStyle, e.Avatar, e.Message
        );
        AvatarChat?.Invoke(e);
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
        var roomData = e.Packet.Read<RoomData>();

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

    [Intercept(~ClientType.Shockwave)]
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

    [Intercept(~ClientType.Shockwave)]
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
    [Intercept(~ClientType.Shockwave)]
    [InterceptIn("f:" + nameof(In.HeightMap))]
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

        Heightmap heightmap = e.Packet.Read<Heightmap>();
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

        FloorPlan floorPlan = e.Packet.Read<FloorPlan>();
        _currentRoom.FloorPlan = floorPlan;

        _logger.LogTrace("Received floor heightmap. (size:{width}x{length})", floorPlan.Width, floorPlan.Length);

        if (Interceptor.Session.Is(ClientType.Shockwave))
        {
            _gotHeightMap = true;
            EnterRoomOnceLoaded();
        }
    }

    [Intercept(~ClientType.Shockwave)]
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

    [Intercept(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.RoomChatSettings))]
    private void HandleRoomChatSettings(Intercept e)
    {
        if (_currentRoom is null)
        {
            return;
        }

        _currentRoom.Data.ChatSettings = e.Packet.Read<ChatSettings>();

        OnRoomDataUpdated(_currentRoom.Data);
    }

    [Intercept(~ClientType.Shockwave)]
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
    [Intercept(~ClientType.Shockwave)]
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
    [Intercept]
    void HandleFloorItems(FloorItemsMsg items)
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

    [Intercept]
    void HandleFloorItemAdded(FloorItemAddedMsg added)
    {
        if (!IsInRoom)
        {
            _logger.LogDebug($"[{nameof(HandleFloorItemAdded)}] Not in room.");
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogWarning($"[{nameof(HandleFloorItemAdded)}] Current room is null.");
            return;
        }

        if (_currentRoom.FloorItems.TryAdd(added.Item.Id, added.Item))
        {
            OnFloorItemAdded(added.Item);
        }
        else
        {
            _logger.LogError($"[{nameof(HandleFloorItemAdded)}] Failed to add floor item {{itemId}}.", added.Item.Id);
        }
    }

    [Intercept]
    void HandleFloorItemRemoved(FloorItemRemovedMsg removed)
    {
        if (!IsInRoom)
        {
            _logger.LogDebug("[{method}] Not in room.", nameof(HandleFloorItemRemoved));
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogWarning("[{method}] Current room is null.", nameof(HandleFloorItemRemoved));
            return;
        }

        if (_currentRoom.FloorItems.TryRemove(removed.Id, out FloorItem? item))
        {
            OnFloorItemRemoved(item);
        }
        else
        {
            _logger.LogError("[{method}] Failed to remove item {id} from the dictionary", nameof(HandleFloorItemRemoved), removed.Id);
        }
    }

    [Intercept]
    void HandleFloorItemUpdated(FloorItemUpdatedMsg updated)
    {
        if (!IsInRoom)
        {
            _logger.LogDebug("[{method}] Not in room.", nameof(HandleFloorItemUpdated));
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogWarning("[{method}] Current room is null.", nameof(HandleFloorItemUpdated));
            return;
        }

        if (_currentRoom.FloorItems.TryGetValue(updated.Item.Id, out FloorItem? previousItem))
        {
            updated.Item.OwnerName = previousItem.OwnerName;
            updated.Item.IsHidden = previousItem.IsHidden;

            if (_currentRoom.FloorItems.TryUpdate(updated.Item.Id, updated.Item, previousItem))
            {
                OnFloorItemUpdated(previousItem, updated.Item);
            }
            else
            {
                _logger.LogError("[{method}] Failed to update floor item {itemId}", nameof(HandleFloorItemUpdated), updated.Item.Id);
            }
        }
        else
        {
            _logger.LogError("[{method}] Failed to find floor item {itemId} to update", nameof(HandleFloorItemUpdated), updated.Item.Id);
        }
    }

    [Intercept]
    void HandleSlideObjectBundle(SlideObjectBundleMsg msg)
    {
        if (!IsInRoom)
        {
            _logger.LogDebug("[{method}] Not in room.", nameof(HandleSlideObjectBundle));
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogWarning("[{method}] Current room is null.", nameof(HandleSlideObjectBundle));
            return;
        }

        var update = msg.Bundle;
        foreach (SlideObject objectUpdate in update.SlideObjects)
        {
            if (_currentRoom.FloorItems.TryGetValue(objectUpdate.Id, out FloorItem? item))
            {
                Tile previousTile = item.Location;
                item.Location = new Tile(update.To, objectUpdate.ToZ);
                OnFloorItemSlide(item, previousTile, update.RollerId);
            }
            else
            {
                _logger.LogError("[{method}] Failed to find floor item {itemId} to update.", nameof(HandleSlideObjectBundle), objectUpdate.Id);
            }
        }

        if (update.Avatar is not null &&
            update.Type is SlideType.WalkingAvatar or SlideType.StandingAvatar)
        {
            if (_currentRoom.Avatars.TryGetValue(update.Avatar.Index, out Avatar? avatar))
            {
                var previousTile = avatar.Location;
                avatar.Location = new Tile(update.To, update.Avatar.ToZ);

                OnAvatarSlide(avatar, previousTile);
            }
            else
            {
                _logger.LogError("Failed to find avatar with index {index} to update.", update.Avatar.Index);
            }
        }
    }

    [Intercept]
    void HandleFloorItemDataUpdated(FloorItemDataUpdatedMsg update)
    {
        if (!IsInRoom)
        {
            _logger.LogDebug("[{method}] Not in room.", nameof(HandleFloorItemDataUpdated));
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogWarning("[{method}] Current room is null.", nameof(HandleFloorItemDataUpdated));
            return;
        }

        if (!_currentRoom.FloorItems.TryGetValue(update.Id, out FloorItem? item))
        {
            _logger.LogError("[{method}] Unable to find floor item {id} to update.", nameof(HandleFloorItemDataUpdated), update.Id);
            return;
        }

        IItemData previousData = item.Data;
        item.Data = update.Data;

        OnFloorItemDataUpdated(item, previousData);
    }

    [Intercept]
    void HandleFloorItemsDataUpdated(FloorItemsDataUpdatedMsg msg)
    {
        if (!IsInRoom)
        {
            _logger.LogDebug("[{method}] Not in room.", nameof(HandleFloorItemsDataUpdated));
            return;
        }

        if (_currentRoom is null)
        {
            _logger.LogWarning("[{method}] Current room is null.", nameof(HandleFloorItemsDataUpdated));
            return;
        }

        foreach (var (id, data) in msg.Updates)
        {
            if (!_currentRoom.FloorItems.TryGetValue(id, out FloorItem? item))
            {
                _logger.LogError("[{method}] Failed to find floor item {id} to update.", id, nameof(HandleFloorItemsDataUpdated));
                continue;
            }

            ItemData previousData = item.Data;
            item.Data = data;

            OnFloorItemDataUpdated(item, previousData);
        }
    }

    [Intercept]
    void HandleDiceValue(DiceValueMsg dice)
    {
        if (!IsInRoom || _currentRoom is null) return;

        if (!_currentRoom.FloorItems.TryGetValue(dice.Id, out FloorItem? item))
        {
            _logger.LogError("[{method}] Failed to find floor item {id} to update.", nameof(HandleDiceValue), dice.Id);
            return;
        }

        int previousValue = item.Data.State;
        item.Data.Value = $"{dice.Value}";

        OnDiceUpdated(item, previousValue, dice.Value);
    }
    #endregion

    #region - Wall item handlers -
    [Intercept]
    void HandleItems(WallItemsMsg items)
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

    [Intercept]
    void HandleItemAdd(WallItemAddedMsg added)
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

        if (_currentRoom.WallItems.TryAdd(added.Item.Id, added.Item))
        {
            OnWallItemAdded(added.Item);
        }
        else
        {
            _logger.LogError("Failed to add wall item {itemId}.", added.Item.Id);
        }
    }

    [Intercept]
    void HandleItemUpdate(WallItemUpdatedMsg updated)
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

        WallItem updatedItem = updated.Item;
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
    }

    [Intercept]
    void HandleItemRemove(WallItemRemovedMsg removed)
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

        if (_currentRoom.WallItems.TryRemove(removed.Id, out WallItem? item))
        {
            OnWallItemRemoved(item);
        }
        else
        {
            Debug.Log($"failed to remove item {removed.Id} from the dictionary");
        }
    }
    #endregion

    #region - Avatar handlers -
    [Intercept]
    void HandleAvatarsAdded(AvatarsAddedMsg avatars)
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

        List<Avatar> newAvatars = [];

        foreach (Avatar avatar in avatars)
        {
            if (_currentRoom.Avatars.TryAdd(avatar.Index, avatar))
            {
                newAvatars.Add(avatar);
                OnAvatarAdded(avatar);
            }
            else
            {
                _logger.LogError("Failed to add avatar {index} {name} (id:{id})", avatar.Index, avatar.Name, avatar.Id);
            }
        }

        if (newAvatars.Count > 0)
        {
            OnAvatarsAdded(newAvatars);
        }

        if (IsLoadingRoom && !_gotUsers && Interceptor.Session.Is(ClientType.Shockwave))
        {
            _gotUsers = true;
            EnterRoomOnceLoaded();
        }
    }

    [Intercept]
    void HandleAvatarRemoved(AvatarRemovedMsg removed)
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

        if (_currentRoom.Avatars.TryRemove(removed.Index, out Avatar? avatar))
        {
            OnAvatarRemoved(avatar);
        }
        else
        {
            _logger.LogError("Failed to remove avatar with index {index}", removed.Index);
        }
    }

    [Intercept]
    void HandlerUserUpdate(AvatarsUpdatedMsg updates)
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

        var updatedAvatars = new List<IAvatar>();

        foreach (var update in updates)
        {
            if (!_currentRoom.Avatars.TryGetValue(update.Index, out Avatar? avatar))
            {
                _logger.LogError("Failed to find avatar with index {index} to update", update.Index);
                continue;
            }

            avatar.Update(update);
            updatedAvatars.Add(avatar);

            OnAvatarUpdated(avatar);
        }

        OnAvatarsUpdated(updatedAvatars);
    }

    [Intercept]
    void HandleWiredMovements(WiredMovementsMsg movements)
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

        foreach (var movement in movements)
        {
            switch (movement)
            {
                case UserWiredMovement m:
                    if (_currentRoom.Avatars.TryGetValue(m.UserIndex, out Avatar? avatar))
                        avatar.Location = m.Destination;
                    break;
                case FloorItemWiredMovement m:
                    if (_currentRoom.FloorItems.TryGetValue(m.FurniId, out FloorItem? item))
                        item.Location = m.Destination;
                    break;
                case WallItemWiredMovement m:
                    if (_currentRoom.WallItems.TryGetValue(m.ItemId, out WallItem? wallItem))
                        wallItem.Location = m.Destination;
                    break;
            }
        }

        OnWiredMovements(movements);
    }

    // TODO: check
    [Intercept(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.UserChange))]
    void HandleUserChange(Intercept e)
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
        if (_currentRoom.Avatars.TryGetValue(index, out Avatar? avatar))
        {
            string previousFigure = avatar.Figure;
            Gender previousGender;
            string previousMotto = avatar.Motto;
            int previousAchievementScore = 0;

            string updatedFigure = e.Packet.Read<string>();
            Gender updatedGender = H.ToGender(e.Packet.Read<string>());
            string updatedMotto = e.Packet.Read<string>();
            int updatedAchievementScore = e.Packet.Read<int>();

            avatar.Figure = updatedFigure;
            avatar.Motto = updatedMotto;

            if (avatar is RoomUser user)
            {
                previousGender = user.Gender;
                user.Gender = updatedGender;
                previousAchievementScore = user.AchievementScore;
                user.AchievementScore = updatedAchievementScore;
            }
            else if (avatar is Bot bot)
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

            OnAvatarDataUpdated(avatar,
                previousFigure, previousGender,
                previousMotto, previousAchievementScore
            );
        }
        else
        {
            _logger.LogError("Failed to find avatar with index {index} to update.", index);
        }
    }

    [Intercept(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.UserNameChanged))]
    void HandleUserNameChanged(Intercept e)
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

        if (_currentRoom.Avatars.TryGetValue(index, out Avatar? avatar))
        {
            string previousName = avatar.Name;
            avatar.Name = newName;
            OnAvatarNameChanged(avatar, previousName);
        }
        else
        {
            _logger.LogError("Failed to find avatar with index {index} to update.", index);
        }
    }

    [Intercept]
    void HandleAvatarIdle(AvatarIdleMsg msg)
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

        if (_currentRoom.Avatars.TryGetValue(msg.Index, out Avatar? avatar))
        {
            bool wasIdle = avatar.IsIdle;
            avatar.IsIdle = msg.Idle;
            OnAvatarIdle(avatar, wasIdle);
        }
        else
        {
            _logger.LogError("Failed to find avatar with index {index} to update.", msg.Index);
        }
    }

    [Intercept]
    void HandleDance(AvatarDanceMsg msg)
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

        if (_currentRoom.Avatars.TryGetValue(msg.Index, out Avatar? avatar))
        {
            Dances previousDance = avatar.Dance;
            avatar.Dance = msg.Dance;
            OnAvatarDance(avatar, previousDance);
        }
        else
        {
            _logger.LogError("Failed to find avatar with index {index} to update.", msg.Index);
            return;
        }
    }

    [Intercept]
    void HandleExpression(AvatarActionMsg msg)
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

        if (_currentRoom.Avatars.TryGetValue(msg.Index, out Avatar? avatar))
        {
            OnAvatarAction(avatar, msg.Action);
        }
        else
        {
            _logger.LogError("Failed to find avatar with index {index} to update.", msg.Index);
        }
    }

    [Intercept]
    void HandleCarryObject(AvatarHandItemMsg msg)
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

        if (_currentRoom.Avatars.TryGetValue(msg.Index, out Avatar? avatar))
        {
            int previousItem = avatar.HandItem;
            avatar.HandItem = msg.Item;
            OnAvatarHandItem(avatar, previousItem);
        }
        else
        {
            _logger.LogError("Failed to find avatar with index {index} to update.", msg.Index);
        }
    }

    [Intercept]
    void HandleAvatarEffect(AvatarEffectMsg msg)
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

        if (_currentRoom.Avatars.TryGetValue(msg.Index, out Avatar? avatar))
        {
            int previousEffect = avatar.Effect;
            avatar.Effect = msg.Effect;
            OnAvatarEffect(avatar, previousEffect);
        }
        else
        {
            _logger.LogError("Failed to find avatar with index {index} to update.", msg.Index);
        }
    }

    [Intercept]
    void HandleUserTyping(AvatarTypingMsg msg)
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

        if (_currentRoom.Avatars.TryGetValue(msg.Index, out Avatar? avatar))
        {
            bool wasTyping = avatar.IsTyping;
            avatar.IsTyping = msg.Typing;
            OnAvatarTyping(avatar, wasTyping);
        }
        else
        {
            _logger.LogError("Failed to find avatar with index {index} to update.", msg.Index);
        }
    }

    [Intercept]
    void HandleChat(Intercept<AvatarChatMsg> e)
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

        var chat = e.Msg;

        if (!_currentRoom.Avatars.TryGetValue(chat.Index, out Avatar? avatar))
        {
            _logger.LogError("Failed to find avatar with index {index}.", chat.Index);
            return;
        }

        AvatarChatEventArgs chatEventArgs = new(avatar, chat.Type, chat.Message, chat.Style);
        OnAvatarChat(chatEventArgs);

        if (chatEventArgs.IsBlocked || avatar.IsHidden) e.Block();
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

    public void Place(Id itemId, WallLocation location)
        => Interceptor.Send(Out.PlaceObject, itemId, location.Wall.X, location.Wall.Y, location.Offset.X, location.Offset.Y); // Orientation ?
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

    public void Move(Id wallItemId, WallLocation loc) => Interceptor.Send(
        Out.MoveWallItem, wallItemId,
        loc.Wall.X, loc.Wall.Y,
        loc.Offset.X, loc.Offset.Y,
        loc.Orientation.Value.ToString()
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
