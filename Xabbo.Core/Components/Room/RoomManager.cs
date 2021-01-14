using System;
using System.Collections.Generic;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    public class RoomManager : XabboComponent
    {
        public enum Features { Paint, DoorTile, HeightMap, FloorPlan, Rating, Permissions }

        private readonly Dictionary<long, RoomData> _roomDataCache = new Dictionary<long, RoomData>();

        public bool IsRingingDoorbell { get; private set; }
        public bool IsInQueue { get; private set; }
        public int QueuePosition { get; private set; }
        // TODO public bool IsSpectating { get; private set; }
        public bool IsLoadingRoom { get; private set; }
        public bool IsInRoom { get; private set; }

        public long Id { get; private set; }
        public string? Model { get; private set; }
        public string? Floor { get; private set; }
        public string? Wallpaper { get; private set; }
        public string? Landscape { get; private set; }
        public RoomData? Data { get; private set; }

        public Tile DoorTile { get; private set; }
        public int EntryDirection { get; private set; }
        public FloorPlan? Tiles { get; private set; }
        public Heightmap? Heightmap { get; private set; }

        public int RightsLevel { get; private set; }
        public bool HasRights => RightsLevel > 0;
        public bool IsOwner { get; private set; }

        public bool CanMute => CheckPermission(Data?.Moderation.WhoCanMute);
        public bool CanKick => CheckPermission(Data?.Moderation.WhoCanKick);
        public bool CanBan => CheckPermission(Data?.Moderation.WhoCanBan);

        #region - Events -
        public event EventHandler? RingingDoorbell;
        public event EventHandler? DoorbellNoAnswer;
        public event EventHandler? EnteredQueue;
        public event EventHandler? QueuePositionUpdated;
        public event EventHandler? Entering;
        public event EventHandler? Entered;
        public event EventHandler? Left;
        public event EventHandler? Kicked;
        public event EventHandler? RightsUpdated;
        public event EventHandler<RoomDataEventArgs>? RoomDataUpdated;
        public event EventHandler<DoorbellEventArgs>? Doorbell;

        protected virtual void OnRingingDoorbell() => RingingDoorbell?.Invoke(this, EventArgs.Empty);
        protected virtual void OnDoorbellNoAnswer() => DoorbellNoAnswer?.Invoke(this, EventArgs.Empty);

        protected virtual void OnEnteredQueue() => EnteredQueue?.Invoke(this, EventArgs.Empty);
        protected virtual void OnQueuePositionUpdated() => QueuePositionUpdated?.Invoke(this, EventArgs.Empty);

        protected virtual void OnEnteringRoom() => Entering?.Invoke(this, EventArgs.Empty);
        protected virtual void OnEnteredRoom() => Entered?.Invoke(this, EventArgs.Empty);
        protected virtual void OnLeftRoom() => Left?.Invoke(this, EventArgs.Empty);
        protected virtual void OnKicked() => Kicked?.Invoke(this, EventArgs.Empty);

        protected virtual void OnRightsUpdated() => RightsUpdated?.Invoke(this, EventArgs.Empty);

        protected virtual void OnRoomDataUpdated(RoomData roomData)
            => RoomDataUpdated?.Invoke(this, new RoomDataEventArgs(roomData));

        protected virtual void OnDoorbell(DoorbellEventArgs e) => Doorbell?.Invoke(this, e);
        #endregion

        protected override void OnInitialize() { }

        private void Reset()
        {
            IsRingingDoorbell =
            IsInQueue =
            IsLoadingRoom =
            IsInRoom = false;
            QueuePosition = 0;

            Model = null;
            Data = null;

            Floor =
            Wallpaper =
            Landscape = null;
            
            DoorTile = default;
            Tiles = null;
            Heightmap = null;

            RightsLevel = 0;
            IsOwner = false;

            OnLeftRoom();
        }

        private bool CheckPermission(ModerationPermissions? permissions)
        {
            if (!IsInRoom) return false;

            switch (permissions)
            {
                case ModerationPermissions.OwnerOnly: return IsOwner;
                case ModerationPermissions.RightsHolders: return RightsLevel > 0;
                case ModerationPermissions.AllUsers: return true;
                default: return false;
            }
        }

        [Receive(nameof(Incoming.GetGuestRoomResult))]
        private void HandleGetGuestRoomResult(IReadOnlyPacket packet)
        {
            var roomData = RoomData.Parse(packet);
            _roomDataCache[roomData.Id] = roomData;

            if (IsInRoom && roomData.Id == Id)
            {
                DebugUtil.Log("room data updated");

                Data = roomData;
                OnRoomDataUpdated(roomData);
            }
        }

        [Receive(nameof(Incoming.OpenConnectionConfirmation))]
        private void HandleOpenConnectionConfirmation(IReadOnlyPacket packet)
        {
            if (IsRingingDoorbell || IsInQueue || IsLoadingRoom || IsInRoom)
            {
                DebugUtil.Log($"leaving current room ({Id})");
                Reset();
            }

            Id = packet.ReadLong();

            if (_roomDataCache.TryGetValue(Id, out RoomData? data))
            {
                Data = data;
                DebugUtil.Log("loaded room data from cache");
            }
        }

        // @Update [InterceptIn("DoorbellAddUser"), RequiredOut("HandleDoorbell")]
        private async void HandleDoorbellAddUser(InterceptArgs e)
        {
            string name = e.Packet.ReadString();
            if (string.IsNullOrEmpty(name))
            {
                DebugUtil.Log("ringing doorbell");

                IsRingingDoorbell = true;
                OnRingingDoorbell();
            }
            else
            {
                var doorbellArgs = new DoorbellEventArgs(name);
                OnDoorbell(doorbellArgs);

                if (doorbellArgs.IsAccepted.HasValue)
                {
                    e.Block();
                    // @Update await SendAsync(Out.HandleDoorbell, name, doorbellArgs.IsAccepted.Value);
                }
            }
        }

        // TODO No more doorbell in unity
        private void HandleHideDoorbell(IReadOnlyPacket packet)
        {
            int roomId = packet.ReadInt();
            if (roomId != Id)
            {
                DebugUtil.Log("room id mismatch");
                return;
            }

            string user = null;
            if (packet.CanReadString())
                user = packet.ReadString();

            if (string.IsNullOrEmpty(user))
            {
                DebugUtil.Log("no answer");
                IsRingingDoorbell = false;
                OnDoorbellNoAnswer();
            }
        }

        [Receive(nameof(Incoming.RoomQueueStatus))]
        private void HandleRoomQueueStatus(IReadOnlyPacket packet)
        {
            packet.ReadLong(); // room id
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
                DebugUtil.Log($"leaving current room ({Id})");
                Reset();
            }

            IsRingingDoorbell =
            IsInQueue = false;
            QueuePosition = 0;

            Model = packet.ReadString();
            Id = packet.ReadLong();

            IsLoadingRoom = true;

            DebugUtil.Log("entering room");
            OnEnteringRoom();
        }

        [Receive(nameof(Incoming.FlatProperty))]
        private void HandleFlatProperty(IReadOnlyPacket packet)
        {
            string key = packet.ReadString();
            string value = packet.ReadString();
            switch (key)
            {
                case "floor": Floor = value; break;
                case "wallpaper": Wallpaper = value; break;
                case "landscape": Landscape = value; break;
                default: DebugUtil.Log($"unknown paint type: {key}"); break;
            }
        }

        [Receive(nameof(Incoming.YouAreController))]
        private void HandleYouAreController(IReadOnlyPacket packet)
        {
            long roomId = packet.ReadLong();
            if (roomId != Id)
            {
                DebugUtil.Log($"room id mismatch: {roomId}");
                return;
            }

            RightsLevel = packet.ReadInt();
            OnRightsUpdated();
        }

        [Receive(nameof(Incoming.YouAreNotController))]
        private void HandleRoomNoRights(IReadOnlyPacket packet)
        {
            long roomId = packet.ReadLong();
            if (roomId != Id)
            {
                DebugUtil.Log($"room id mismatch: {roomId}");
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
                DebugUtil.Log("not entering room");
                return;
            }

            int x = packet.ReadInt();
            int y = packet.ReadInt();
            int dir = packet.ReadInt();

            DoorTile = new Tile(x, y, 0);
            EntryDirection = dir;

            DebugUtil.Log("loaded door tile");
        }

        [Receive(nameof(Incoming.StackingHeightmap))]
        private void HandleStackingHeightmap(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            Heightmap = Heightmap.Parse(packet);
        }

        [Receive(nameof(Incoming.StackingHeightmapDiff))]
        private void HandleStackingHeightmapDiff(IReadOnlyPacket packet)
        {
            if (Heightmap == null) return;

            int n = packet.ReadByte();
            for (int i = 0; i < n; i++)
            {
                int x = packet.ReadByte();
                int y = packet.ReadByte();
                Heightmap[x, y] = packet.ReadShort();
            }
        }

        [Receive(nameof(Incoming.FloorHeightmap))]
        private void HandleFloorHeightmap(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            Tiles = FloorPlan.Parse(packet);

            DebugUtil.Log("loaded floor plan");
        }

        [Receive(nameof(Incoming.RoomEntryInfo))]
        private void HandleRoomEntryInfo(IReadOnlyPacket packet)
        {
            if (!IsLoadingRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            long roomId = packet.ReadLong();
            if (roomId != Id)
            {
                DebugUtil.Log("room id mismatch");
                return;
            }

            IsOwner = packet.ReadBool();
            IsLoadingRoom = false;
            IsInRoom = true;

            if (_roomDataCache.TryGetValue(roomId, out RoomData? roomData))
            {
                DebugUtil.Log("loaded room data from cache");
                Data = roomData;
            }

            DebugUtil.Log("entered room");
            OnEnteredRoom();
        }

        [Receive(nameof(Incoming.CloseConnection))]
        private void HandleCloseConnection(IReadOnlyPacket packet)
        {
            if (IsRingingDoorbell || IsInQueue || IsLoadingRoom || IsInRoom)
            {
                DebugUtil.Log($"leaving current room ({Id})");
                Reset();
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
    }
}
