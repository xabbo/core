using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Interceptor;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game
{
    public class RoomManager : GameStateManager
    {
        private readonly Dictionary<long, RoomData> _roomDataCache = new Dictionary<long, RoomData>();

        private Room? _room;

        private long _roomId;
        private string _model = string.Empty;
        private string _floor = string.Empty;
        private string _wallpaper = string.Empty;
        private string _landscape = string.Empty;

        private RoomData? _roomData;

        private Tile _doorTile;
        private int _entryDirection;
        private FloorPlan? _floorPlan;
        private Heightmap? _heightmap;

        private ConcurrentDictionary<long, IFloorItem> _floorItems;
        private ConcurrentDictionary<long, IWallItem> _wallItems;

        private ConcurrentDictionary<long, IEntity> _entities;
        private ConcurrentDictionary<int, IEntity> _entityByIndex;
        private ConcurrentDictionary<string, IEntity> _entityByName;

        public bool IsRingingDoorbell { get; private set; }
        public bool IsInQueue { get; private set; }
        public int QueuePosition { get; private set; }
        // TODO public bool IsSpectating { get; private set; }
        public bool IsLoadingRoom { get; private set; }
        public bool IsInRoom { get; private set; }

        public IRoom? Room => _room;
        public IRoomData? Data => _roomData;

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
        /// <summary>
        /// Invoked when the user's position in the queue is updated.
        /// </summary>
        public event EventHandler? QueuePositionUpdated;
        /// <summary>
        /// Invoked when the user enters a room and begins loading the room state.
        /// </summary>
        public event EventHandler? Entering;
        /// <summary>
        /// Invoked after the user has entered the room and the room state is fully loaded.
        /// </summary>
        public event EventHandler? Entered;
        /// <summary>
        /// Invoked when the room data is updated.
        /// </summary>
        public event EventHandler<RoomDataEventArgs>? RoomDataUpdated;
        /// <summary>
        /// Invoked when the user leaves a room.
        /// </summary>
        public event EventHandler? Left;
        /// <summary>
        /// Invoked when the user is kicked from the room.
        /// The user still remains in the room at this point until their avatar leaves through the door.
        /// </summary>
        public event EventHandler? Kicked;
        /// <summary>
        /// Invoked when the user's rights to the room are updated.
        /// </summary>
        public event EventHandler? RightsUpdated;

        protected virtual void OnEnteredQueue() => EnteredQueue?.Invoke(this, EventArgs.Empty);
        protected virtual void OnQueuePositionUpdated() => QueuePositionUpdated?.Invoke(this, EventArgs.Empty);
        protected virtual void OnEnteringRoom() => Entering?.Invoke(this, EventArgs.Empty);
        protected virtual void OnEnteredRoom() => Entered?.Invoke(this, EventArgs.Empty);
        protected virtual void OnRoomDataUpdated(RoomData roomData)
            => RoomDataUpdated?.Invoke(this, new RoomDataEventArgs(roomData));
        protected virtual void OnLeftRoom() => Left?.Invoke(this, EventArgs.Empty);
        protected virtual void OnKicked() => Kicked?.Invoke(this, EventArgs.Empty);
        protected virtual void OnRightsUpdated() => RightsUpdated?.Invoke(this, EventArgs.Empty);
        #endregion

        #region - Furni events -
        /// <summary>
        /// Invoked when a floor item is added to the room.
        /// </summary>
        public event EventHandler<FloorItemEventArgs>? FloorItemAdded;
        /// <summary>
        /// Invoked when a floor item is moved or rotated.
        /// </summary>
        public event EventHandler<FloorItemUpdatedEventArgs>? FloorItemUpdated;
        /// <summary>
        /// Invoked when a floor item's data is updated.
        /// </summary>
        public event EventHandler<FloorItemDataUpdatedEventArgs>? FloorItemDataUpdated;
        /// <summary>
        /// Invoked when a floor item slides due to a roller or wired event.
        /// </summary>
        public event EventHandler<FloorItemSlideEventArgs>? FloorItemSlide;
        /// <summary>
        /// Invoked when a floor item is removed from the room.
        /// </summary>
        public event EventHandler<FloorItemEventArgs>? FloorItemRemoved;

        /// <summary>
        /// Invoked when a wall item is added to the room.
        /// </summary>
        public event EventHandler<WallItemEventArgs>? WallItemAdded;
        /// <summary>
        /// Invoked when a wall item is moved or its data is updated.
        /// </summary>
        public event EventHandler<WallItemUpdatedEventArgs>? WallItemUpdated;
        /// <summary>
        /// Invoked when a wall item is removed from the room.
        /// </summary>
        public event EventHandler<WallItemEventArgs>? WallItemRemoved;

        /// <summary>
        /// Invoked when a furni's visibility is toggled using <see cref="Hide(IFurni)"/> or <see cref="Show(IFurni)"/>.
        /// </summary>
        public event EventHandler<FurniEventArgs>? FurniVisibilityToggled;

        protected virtual void OnFloorItemAdded(IFloorItem item)
            => FloorItemAdded?.Invoke(this, new FloorItemEventArgs(item));
        protected virtual void OnFloorItemUpdated(IFloorItem previousItem, IFloorItem updatedItem)
            => FloorItemUpdated?.Invoke(this, new FloorItemUpdatedEventArgs(previousItem, updatedItem));
        protected virtual void OnFloorItemDataUpdated(IFloorItem item, IItemData previousData)
            => FloorItemDataUpdated?.Invoke(this, new FloorItemDataUpdatedEventArgs(item, previousData));
        protected virtual void OnFloorItemSlide(IFloorItem item, Tile previousTile, long rollerId)
            => FloorItemSlide?.Invoke(this, new FloorItemSlideEventArgs(item, previousTile, rollerId));
        protected virtual void OnFloorItemRemoved(IFloorItem item)
            => FloorItemRemoved?.Invoke(this, new FloorItemEventArgs(item));

        protected virtual void OnWallItemAdded(IWallItem item)
            => WallItemAdded?.Invoke(this, new WallItemEventArgs(item));
        protected virtual void OnWallItemUpdated(IWallItem previousItem, IWallItem updatedItem)
            => WallItemUpdated?.Invoke(this, new WallItemUpdatedEventArgs(previousItem, updatedItem));
        protected virtual void OnWallItemRemoved(IWallItem item)
            => WallItemRemoved?.Invoke(this, new WallItemEventArgs(item));

        protected virtual void OnFurniVisibilityToggled(IFurni furni)
            => FurniVisibilityToggled?.Invoke(this, new FurniEventArgs(furni));
        #endregion

        #region - Entity events -

        #endregion

#pragma warning disable CS8618 // Non-nullable fields are initialized in the ResetRoom() method.
        public RoomManager(IInterceptor interceptor)
            : base(interceptor)
        {
            ResetRoom();
        }
#pragma warning restore CS8618

        #region - Room handler -
        private void ResetRoom() 
        {
            IsInQueue =
            IsLoadingRoom =
            IsInRoom = false;
            QueuePosition = 0;

            RightsLevel = 0;
            IsOwner = false;

            _room = null;
            _roomId = -1;
            _model = 
            _floor =
            _wallpaper = 
            _landscape = string.Empty;

            _roomData = null;

            _floorPlan = null;
            _heightmap = null;

            _floorItems = new ConcurrentDictionary<long, IFloorItem>();
            _wallItems = new ConcurrentDictionary<long, IWallItem>();

            _entities = new ConcurrentDictionary<long, IEntity>();
            _entityByIndex = new ConcurrentDictionary<int, IEntity>();
            _entityByName = new ConcurrentDictionary<string, IEntity>();

            OnLeftRoom();
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

        [Receive(nameof(Incoming.GetGuestRoomResult))]
        private void HandleGetGuestRoomResult(IReadOnlyPacket packet)
        {
            var roomData = RoomData.Parse(packet);
            _roomDataCache[roomData.Id] = roomData;

            if (IsInRoom && roomData.Id == _roomId)
            {
                DebugUtil.Log("room data updated");

                _roomData = roomData;
                if (_room is not null)
                    _room.Data = roomData;

                OnRoomDataUpdated(roomData);
            }
        }

        [Receive(nameof(Incoming.OpenConnectionConfirmation))]
        private void HandleOpenConnectionConfirmation(IReadOnlyPacket packet)
        {
            if (IsInQueue || IsLoadingRoom || IsInRoom)
            {
                DebugUtil.Log($"leaving current room ({_roomId})");
                ResetRoom();
            }

            _roomId = packet.ReadLegacyLong();

            if (_roomDataCache.TryGetValue(_roomId, out RoomData? data))
            {
                _roomData = data;
                DebugUtil.Log("loaded room data from cache");
            }
        }

        [Receive(nameof(Incoming.RoomQueueStatus))]
        private void HandleRoomQueueStatus(IReadOnlyPacket packet)
        {
            long roomId = packet.ReadLegacyLong();
            if (roomId != _roomId) return;

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
                    DebugUtil.Log($"entered queue: {QueuePosition}");
                    OnEnteredQueue();
                }
                else
                {
                    DebugUtil.Log($"queue position updated: {QueuePosition}");
                    OnQueuePositionUpdated();
                }
            }
        }

        [Receive(nameof(Incoming.RoomReady))]
        private void HandleRoomReady(IReadOnlyPacket packet)
        {
            if (IsLoadingRoom || IsInRoom)
            {
                DebugUtil.Log($"leaving current room ({_roomId})");
                ResetRoom();
            }

            if (IsInQueue)
            {
                IsInQueue = false;
                QueuePosition = 0;
            }

            _model = packet.ReadString();
            _roomId = packet.ReadLegacyLong();

            IsLoadingRoom = true;

            DebugUtil.Log("entering room");
            OnEnteringRoom();
        }

        [Receive(nameof(Incoming.FlatProperty))]
        private void HandleFlatProperty(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            string key = packet.ReadString();
            string value = packet.ReadString();
            switch (key)
            {
                case "floor": _floor = value; break;
                case "wallpaper": _wallpaper = value; break;
                case "landscape": _landscape = value; break;
                default: DebugUtil.Log($"unknown paint type: {key}"); break;
            }

            // TODO Add a property updated event if already in room
        }

        [Receive(nameof(Incoming.YouAreController))]
        private void HandleYouAreController(IReadOnlyPacket packet)
        {
            long roomId = packet.ReadLegacyLong();
            if (roomId != _roomId) return;

            RightsLevel = packet.ReadInt();
            OnRightsUpdated();
        }

        [Receive(nameof(Incoming.YouAreNotController))]
        private void HandleRoomNoRights(IReadOnlyPacket packet)
        {
            long roomId = packet.ReadLegacyLong();
            if (roomId != _roomId) return;

            RightsLevel = 0;
            OnRightsUpdated();
        }

        [Receive(nameof(Incoming.RoomEntryTile))]
        private void HandleRoomEntryTile(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom) return;

            int x = packet.ReadInt();
            int y = packet.ReadInt();
            int dir = packet.ReadInt();

            _doorTile = new Tile(x, y, 0);
            _entryDirection = dir;
        }

        [Receive(nameof(Incoming.StackingHeightmap))]
        private void HandleStackingHeightmap(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom) return;

            _heightmap = Heightmap.Parse(packet);
        }

        [Receive(nameof(Incoming.StackingHeightmapDiff))]
        private void HandleStackingHeightmapDiff(IReadOnlyPacket packet)
        {
            if (_room is null) return;

            int n = packet.ReadByte();
            for (int i = 0; i < n; i++)
            {
                int x = packet.ReadByte();
                int y = packet.ReadByte();
                _room.Heightmap[x, y] = packet.ReadShort();
            }
        }

        [Receive(nameof(Incoming.FloorHeightmap))]
        private void HandleFloorHeightmap(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom) return;

            _floorPlan = FloorPlan.Parse(packet);
        }

        [Receive(nameof(Incoming.RoomEntryInfo))]
        private void HandleRoomEntryInfo(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            long roomId = packet.ReadLegacyLong();
            if (roomId != _roomId)
            {
                DebugUtil.Log("room id mismatch");
                return;
            }

            if (_roomDataCache.TryGetValue(roomId, out RoomData? roomData))
            {
                DebugUtil.Log("loaded room data from cache");
                _roomData = roomData;
            }

            if (_floorPlan is null ||
                _heightmap is null ||
                _floorItems is null ||
                _wallItems is null ||
                _entities is null ||
                _entityByIndex is null ||
                _entityByName is null)
            {
                throw new NullReferenceException("");
            }

            _room = new Room(_roomId, _roomData,
                _model, _doorTile, _entryDirection, _floorPlan, _heightmap,
                _floorItems, _wallItems,
                _entities, _entityByIndex, _entityByName
            );

            IsOwner = packet.ReadBool();
            IsLoadingRoom = false;
            IsInRoom = true;

            DebugUtil.Log("entered room");
            OnEnteredRoom();
        }

        [Receive(nameof(Incoming.CloseConnection))]
        private void HandleCloseConnection(IReadOnlyPacket packet)
        {
            if (_room is null) return;

            if (IsRingingDoorbell || IsInQueue || IsLoadingRoom || IsInRoom)
            {
                DebugUtil.Log($"leaving current room ({_room.Id})");
                ResetRoom();
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

        #region - Furni handler -

        #endregion

        #region - Entity handler -

        #endregion
    }
}
