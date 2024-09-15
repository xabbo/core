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
using System.Runtime.CompilerServices;

namespace Xabbo.Core.Game;

/// <summary>
/// Manages information about the current room, the user's permissions in the room, its furni, avatars and chat.
/// </summary>
[Intercept]
public sealed partial class RoomManager(IInterceptor interceptor, ILoggerFactory? loggerFactory = null)
    : GameStateManager(interceptor)
{
    private readonly ILogger Log = (ILogger?)loggerFactory?.CreateLogger<RoomManager>() ?? NullLogger.Instance;
    private readonly Dictionary<long, RoomData> _roomDataCache = [];

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
        Log.LogDebug("Entered queue. (pos:{QueuePosition})", QueuePosition);
        EnteredQueue?.Invoke();
    }

    /// <summary>
    /// Invoked when the user's position in the queue is updated.
    /// </summary>
    public event Action? QueuePositionUpdated;
    private void OnQueuePositionUpdated()
    {
        Log.LogDebug("Queue position updated. (pos:{QueuePosition})", QueuePosition);
        QueuePositionUpdated?.Invoke();
    }

    /// <summary>
    /// Invoked when the user enters a room and begins loading the room state.
    /// </summary>
    public event Action? Entering;
    private void OnEnteringRoom(long roomId)
    {
        Log.LogDebug("Entering room. (id:{RoomId})", roomId);
        Entering?.Invoke();
    }

    /// <summary>
    /// Invoked after the user has entered the room and the room state is fully loaded.
    /// </summary>
    public event Action<RoomEventArgs>? Entered;
    private void OnEnteredRoom(IRoom room)
    {
        Log.LogInformation("Entered room. (id:{RoomId}, name:{Name})", room.Id, room.Name);
        Entered?.Invoke(new RoomEventArgs(room));
    }

    /// <summary>Invoked when the room data is updated.</summary>
    public event Action<RoomDataEventArgs>? RoomDataUpdated;
    private void OnRoomDataUpdated(RoomData roomData)
    {
        Log.LogDebug("Room data updated. (name:{Name})", roomData.Name);
        RoomDataUpdated?.Invoke(new RoomDataEventArgs(roomData));
    }

    /// <summary>
    /// Invoked when the user leaves a room.
    /// </summary>
    public event Action? Left;
    private void OnLeftRoom()
    {
        Log.LogInformation("Left room.");
        Left?.Invoke();
    }

    /// <summary>
    /// Invoked when the user is kicked from the room.
    /// The user still remains in the room at this point until their avatar leaves through the door.
    /// </summary>
    public event Action? Kicked;
    private void OnKicked()
    {
        Log.LogDebug("Kicked from room.");
        Kicked?.Invoke();
    }

    /// <summary>
    /// Invoked when the user's rights to the room are updated.
    /// </summary>
    public event Action? RightsUpdated;
    private void OnRightsUpdated()
    {
        Log.LogDebug("Rights level updated to {Level}.", RightsLevel);
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

    /// <summary>
    /// Invoked when a floor item is added to the room.
    /// </summary>
    public event Action<FloorItemEventArgs>? FloorItemAdded;

    /// <summary>
    /// Invoked when a floor item is updated.
    /// </summary>
    public event Action<FloorItemUpdatedEventArgs>? FloorItemUpdated;

    /// <summary>
    /// Invoked when a floor item's data is updated.
    /// </summary>
    public event Action<FloorItemDataUpdatedEventArgs>? FloorItemDataUpdated;

    public event Action<DiceUpdatedEventArgs>? DiceUpdated;

    /// <summary>
    /// Invoked when a floor item slides due to a roller or wired update.
    /// </summary>
    public event Action<FloorItemSlideEventArgs>? FloorItemSlide;
    private void OnFloorItemSlide(IFloorItem item, Tile previousTile, Id rollerId)
    {
        Log.LogTrace(
            "Floor item slide. (id:{Id}, rollerId:{RollerId}, {From} -> {To})",
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
        Log.LogTrace("Floor item removed. (id:{Id})", item.Id);
        FloorItemRemoved?.Invoke(new FloorItemEventArgs(item));
    }

    /// <summary>
    /// Invoked when the wall items are loaded.
    /// This may happen multiple times depending on
    /// how many items are in the room.
    /// </summary>
    public event Action<WallItemsEventArgs>? WallItemsLoaded;

    /// <summary>
    /// Invoked when a wall item is added to the room.
    /// </summary>
    public event Action<WallItemEventArgs>? WallItemAdded;

    /// <summary>
    /// Invoked when a wall item is udpated.
    /// </summary>
    public event Action<WallItemUpdatedEventArgs>? WallItemUpdated;

    /// <summary>
    /// Invoked when a wall item is removed from the room.
    /// </summary>
    public event Action<WallItemEventArgs>? WallItemRemoved;

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

    /// <summary>
    /// Invoked when avatars have been added to the room.
    /// </summary>
    public event Action<AvatarsEventArgs>? AvatarsAdded;

    /// <summary>
    /// Invoked when an avatar in the room is updated.
    /// </summary>
    public event Action<AvatarEventArgs>? AvatarUpdated;

    /// <summary>
    /// Invoked when avatars in the room are updated.
    /// </summary>
    public event Action<AvatarsEventArgs>? AvatarsUpdated;
    private void OnAvatarsUpdated(IEnumerable<IAvatar> avatars)
    {
    }

    /// <summary>
    /// Invoked when an avatar slides along a roller.
    /// </summary>
    public event Action<AvatarSlideEventArgs>? AvatarSlide;
    private void OnAvatarSlide(IAvatar avatar, Tile previousTile)
    {
        Log.LogTrace(
            "Avatar slide. ({AvatarName} [{AvatarId}:{AvatarIndex}], {From} -> {To})",
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
        Log.LogTrace(
            "Avatar data updated. ({Name} [{Id}:{Index}])",
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
        Log.LogTrace(
            "Avatar name changed. ({PreviousName} -> {AvatarName} [{AvatarId}:{AvatarIndex}])",
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
        Log.LogTrace(
            "Avatar idle. ({AvatarName} [{AvatarId}:{AvatarIndex}], {WasIdle} -> {IsIdle})",
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
        Log.LogTrace(
            "Avatar dance. ({AvatarName} [{AvatarId}:{AvatarIndex}], {PreviousDance} -> {Dance})",
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
        Log.LogTrace(
            "Avatar hand item. ({AvatarName} [{AvatarId}:{AvatarIndex}], {PreviousItemId} -> {ItemId})",
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
        Log.LogTrace(
            "Avatar effect. ({AvatarName} [{AvatarId}:{AvatarIndex}], {PreviousEffect} -> {Effect})",
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
        Log.LogTrace(
            "Avatar action. ({AvatarName} [{AvatarId}:{AvatarIndex}], action:{Action})",
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
        Log.LogTrace(
            "Avatar typing. ({AvatarName} [{AvatarId}:{AvatarIndex}], {WasTyping} -> {IsTyping})",
            avatar.Name, avatar.Id, avatar.Index, wasTyping, avatar.IsTyping
        );
        AvatarTyping?.Invoke(new AvatarTypingEventArgs(avatar, wasTyping));
    }

    /// <summary>
    /// Invoked when an avatar is removed from the room.
    /// </summary>
    public event Action<AvatarEventArgs>? AvatarRemoved;

    /// <summary>
    /// Invoked when an avatar in the room talks.
    /// </summary>
    public event Action<AvatarChatEventArgs>? AvatarChat;
    private void OnAvatarChat(AvatarChatEventArgs e)
    {
        Log.LogTrace(
            "{Type}({Bubble}) {Avatar} {Message}",
            e.ChatType, e.BubbleStyle, e.Avatar, e.Message
        );
        AvatarChat?.Invoke(e);
    }
    #endregion

    /// <summary>
    /// Retrieves the room data from the cache if it is available.
    /// </summary>
    public bool TryGetRoomData(long roomId, [NotNullWhen(true)] out RoomData? data) => _roomDataCache.TryGetValue(roomId, out data);

    protected override void OnDisconnected() => ResetState();

    private bool EnsureRoom([NotNullWhen(true)] out Room? room)
    {
        room = _currentRoom;
        if (room is null)
            Log.LogDebug("Current room is null.");
        return room is not null;
    }

    private bool EnsureLoadingRoom([NotNullWhen(true)] out Room? room)
    {
        if (!IsLoadingRoom)
        {
            room = null;
            Log.LogDebug("Not loading room.");
            return false;
        }

        return EnsureRoom(out room);
    }

    private bool EnsureInRoom([NotNullWhen(true)] out Room? room)
    {
        if (!IsInRoom)
        {
            room = null;
            Log.LogDebug("Not in room.");
            return false;
        }

        return EnsureRoom(out room);
    }

    private void EnteringRoom(Id id)
    {
        Log.LogDebug("Entering room #{Id}.", id);

        CurrentRoomId = id;

        if (_roomDataCache.TryGetValue(id, out RoomData? data))
        {
            _currentRoomData = data;
            Log.LogDebug("Loaded room data from cache.");
        }
        else
        {
            Log.LogWarning("Failed to load room data from cache.");
        }

        _currentRoom = new Room(id, data!);
    }

    // Checks the load state and enters the room on Shockwave.
    private void EnterRoomOnceLoadedOrigins()
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

    #region - Furni -
    void LoadFloorItems(IEnumerable<FloorItem> items)
    {
        if (!EnsureLoadingRoom(out Room? room))
            return;

        List<FloorItem> newItems = [];

        foreach (FloorItem item in items)
        {
            if (room.FloorItems.TryAdd(item.Id, item))
            {
                newItems.Add(item);
            }
            else
            {
                Log.LogWarning("Failed to add floor item #{Id}.", item.Id);
            }
        }

        if (newItems.Count > 0)
        {
            Log.LogDebug("Loaded {Count} floor items.", newItems.Count);
            FloorItemsLoaded?.Invoke(new FloorItemsEventArgs(items));
        }

        if (Interceptor.Session.Is(ClientType.Shockwave))
        {
            _gotObjects = true;
            EnterRoomOnceLoadedOrigins();
        }
    }

    void AddFloorItem(FloorItem item)
    {
        if (!EnsureInRoom(out Room? room))
            return;

        if (room.FloorItems.TryAdd(item.Id, item))
        {
            Log.LogDebug("Floor item #{Id} added.", item.Id);
            FloorItemAdded?.Invoke(new FloorItemEventArgs(item));
        }
        else
        {
            Log.LogWarning("Failed to add floor item #{Id}.", item.Id);
        }
    }

    void UpdateFloorItem(FloorItem item)
    {
        if (!EnsureInRoom(out Room? room))
            return;

        if (room.FloorItems.TryGetValue(item.Id, out FloorItem? previousItem))
        {
            item.OwnerName = previousItem.OwnerName;
            item.IsHidden = previousItem.IsHidden;

            if (room.FloorItems.TryUpdate(item.Id, item, previousItem))
            {
                Log.LogTrace("Floor item #{Id} updated.", item.Id);
                FloorItemUpdated?.Invoke(new FloorItemUpdatedEventArgs(previousItem, item));
            }
            else
            {
                Log.LogWarning("Failed to update floor item #{Id}.", item.Id);
            }
        }
        else
        {
            Log.LogWarning("Failed to find floor item #{Id} to update.", item.Id);
        }
    }

    void UpdateFloorItemData(IEnumerable<(Id, ItemData)> updates)
    {
        if (!EnsureInRoom(out Room? room))
            return;

        foreach (var (id, data) in updates)
        {
            if (!room.FloorItems.TryGetValue(id, out FloorItem? item))
            {
                Log.LogWarning("Failed to find floor item #{Id} to update.", id);
                continue;
            }

            IItemData previousData = item.Data;
            item.Data = data;

            Log.LogTrace("Floor item #{Id} data updated.", item.Id);
            FloorItemDataUpdated?.Invoke(new FloorItemDataUpdatedEventArgs(item, previousData));
        }
    }

    void UpdateDiceValue(Id id, int value)
    {
        if (!EnsureInRoom(out Room? room))
            return;

        if (!room.FloorItems.TryGetValue(id, out FloorItem? dice))
        {
            Log.LogWarning("Failed to find floor item #{Id} to update.", id);
            return;
        }

        int previousValue = dice.Data.State;
        dice.Data.Value = value.ToString();

        Log.LogTrace("Dice #{Id} value updated. ({PreviousValue} -> {CurrentValue})", id, previousValue, value);
        DiceUpdated?.Invoke(new DiceUpdatedEventArgs(dice, previousValue, value));
    }

    void RemoveFloorItem(Id id)
    {
        if (!EnsureInRoom(out Room? room))
            return;

        if (room.FloorItems.TryRemove(id, out FloorItem? item))
        {
            Log.LogDebug("Floor item #{Id} removed.", item.Id);
            FloorItemRemoved?.Invoke(new FloorItemEventArgs(item));
        }
        else
        {
            Log.LogWarning("Failed to remove floor item #{Id}.", id);
        }

    }
    #endregion

    #region - Wall items -
    void LoadWallItems(IEnumerable<WallItem> items)
    {
        if (!EnsureLoadingRoom(out Room? room))
            return;

        List<WallItem> newItems = [];

        foreach (var item in items)
        {
            if (room.WallItems.TryAdd(item.Id, item))
            {
                newItems.Add(item);
            }
            else
            {
                Log.LogWarning("Failed to add wall item #{Id}.", item.Id);
            }
        }

        if (newItems.Count > 0)
        {
            Log.LogDebug("Loaded {Count} wall items.", newItems.Count);
            WallItemsLoaded?.Invoke(new WallItemsEventArgs(items));
        }

        if (Interceptor.Session.Is(ClientType.Shockwave))
        {
            _gotItems = true;
            EnterRoomOnceLoadedOrigins();
        }
    }

    void AddWallItem(WallItem item)
    {
        if (!EnsureInRoom(out Room? room))
            return;

        if (room.WallItems.TryAdd(item.Id, item))
        {
            Log.LogDebug("Wall item #{Id} added.", item.Id);
            WallItemAdded?.Invoke(new WallItemEventArgs(item));
        }
        else
        {
            Log.LogWarning("Failed to add wall item #{Id}.", item.Id);
        }
    }

    void UpdateWallItem(WallItem item)
    {
        if (!EnsureInRoom(out Room? room))
            return;

        if (room.WallItems.TryGetValue(item.Id, out WallItem? previousItem))
        {
            item.OwnerName = previousItem.OwnerName;
            item.IsHidden = previousItem.IsHidden;

            if (room.WallItems.TryUpdate(item.Id, item, previousItem))
            {
                Log.LogTrace("Wall item #{Id} updated.", item.Id);
                WallItemUpdated?.Invoke(new WallItemUpdatedEventArgs(previousItem, item));
            }
            else
            {
                Log.LogWarning("Failed to update wall item #{Id}.", item.Id);
            }
        }
        else
        {
            Log.LogWarning("Failed to find wall item #{Id} to update.", item.Id);
        }
    }

    void RemoveWallItem(Id id)
    {
        if (!EnsureInRoom(out Room? room))
            return;

        if (room.WallItems.TryRemove(id, out WallItem? item))
        {
            Log.LogDebug("Wall item #{Id} removed.", id);
            WallItemRemoved?.Invoke(new WallItemEventArgs(item));
        }
        else
        {
            Log.LogWarning("Failed to remove wall item #{Id}.", id);
        }
    }
    #endregion

    #region Avatars
    void AddAvatars(IEnumerable<Avatar> avatars)
    {
        if (!IsLoadingRoom && !IsInRoom)
        {
            Log.LogWarning("Not loading or in room.");
            return;
        }

        if (!EnsureRoom(out Room? room))
            return;

        List<Avatar> added = [];

        foreach (Avatar avatar in avatars)
        {
            if (room.Avatars.TryAdd(avatar.Index, avatar))
            {
                added.Add(avatar);
                Log.LogTrace("Added avatar @{Index} '{Name}'.", added[0].Index, added[0].Name);
                AvatarAdded?.Invoke(new AvatarEventArgs(avatar));
            }
            else
            {
                Log.LogWarning("Failed to add avatar @{Index} '{Name}'.", avatar.Index, avatar.Id);
            }
        }

        if (added.Count > 0)
        {
            Log.LogDebug("Added {Count} avatars.", added.Count);
            AvatarsAdded?.Invoke(new AvatarsEventArgs(added));
        }

        if (IsLoadingRoom && !_gotUsers && Interceptor.Session.Is(ClientType.Shockwave))
        {
            _gotUsers = true;
            EnterRoomOnceLoadedOrigins();
        }
    }

    void UpdateAvatars(IEnumerable<AvatarStatusUpdate> updates)
    {
        if (!EnsureInRoom(out Room? room))
            return;

        List<IAvatar> updated = [];

        foreach (var update in updates)
        {
            if (!room.Avatars.TryGetValue(update.Index, out Avatar? avatar))
            {
                Log.LogWarning("Failed to find avatar @{Index} to update.", update.Index);
                continue;
            }

            avatar.Update(update);
            updated.Add(avatar);

            Log.LogTrace("Updated avatar @{Index} '{Name}': '{Status}'.",
                avatar.Index, avatar.Name, update.Status);
            AvatarUpdated?.Invoke(new AvatarEventArgs(avatar));
        }

        AvatarsUpdated?.Invoke(new AvatarsEventArgs(updated));
    }

    void RemoveAvatar(int index)
    {
        if (!EnsureInRoom(out Room? room))
            return;

        if (room.Avatars.TryRemove(index, out Avatar? avatar))
        {
            Log.LogDebug("Avatar @{Index} '{Name}' removed.", avatar.Index, avatar.Index);
            AvatarRemoved?.Invoke(new AvatarEventArgs(avatar));
        }
        else
        {
            Log.LogWarning("Failed to remove avatar @{Index}.", index);
        }
    }
    #endregion

    private void ResetState()
    {
        Log.LogDebug("Resetting room state.");

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
            if (furni is FloorItem floorItem)
            {
                Interceptor.Send(new FloorItemRemovedMsg(floorItem));
            }
            else if (furni is WallItem)
            {
                Interceptor.Send(new WallItemRemovedMsg(furni.Id));
            }
        }
        else
        {
            if (furni is FloorItem floorItem)
                Interceptor.Send(new FloorItemAddedMsg(floorItem));
            else if (furni is WallItem wallItem)
                Interceptor.Send(new WallItemAddedMsg(wallItem));
        }

        OnFurniVisibilityToggled(furni);
        return true;
    }

    public bool ShowFurni(ItemType type, Id id) => SetFurniHidden(type, id, false);
    public bool HideFurni(ItemType type, Id id) => SetFurniHidden(type, id, true);

    public bool ShowFurni(IFurni furni) => SetFurniHidden(furni.Type, furni.Id, false);
    public bool HideFurni(IFurni furni) => SetFurniHidden(furni.Type, furni.Id, true);

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
