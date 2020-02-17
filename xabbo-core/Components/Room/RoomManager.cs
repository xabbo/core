using System;
using System.Collections.Generic;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    public class RoomManager : XabboComponent
    {
        public enum Features { Permissions, DoorTile, HeightMap, FloorPlan }

        private readonly Dictionary<int, RoomData> roomDataCache = new Dictionary<int, RoomData>();

        public bool IsRingingDoorbell { get; private set; }
        public bool IsInQueue { get; private set; }
        public int QueuePosition { get; private set; }
        // TODO public bool IsSpectating { get; private set; }
        public bool IsEnteringRoom { get; private set; }
        public bool IsInRoom { get; private set; }

        public int Id { get; private set; }
        public string Model { get; private set; }
        public RoomData Data { get; private set; }

        public Tile DoorTile { get; private set; }
        public FloorPlan FloorPlan { get; private set; }
        public HeightMap HeightMap { get; private set; }

        public int RightsLevel { get; private set; }
        public bool HasRights => RightsLevel > 0;
        public bool IsOwner { get; private set; }

        #region - Events -
        public event EventHandler RingingDoorbell;
        public event EventHandler EnteredQueue;
        public event EventHandler QueuePositionUpdated;
        public event EventHandler Entering;
        public event EventHandler Entered;
        public event EventHandler Kicked;
        public event EventHandler Left;

        public event EventHandler<RoomDataEventArgs> RoomDataUpdated;

        protected virtual void OnRingingDoorbell() => RingingDoorbell?.Invoke(this, EventArgs.Empty);

        protected virtual void OnEnteredQueue() => EnteredQueue?.Invoke(this, EventArgs.Empty);
        protected virtual void OnQueuePositionUpdated() => QueuePositionUpdated?.Invoke(this, EventArgs.Empty);

        protected virtual void OnEnteringRoom() => Entering?.Invoke(this, EventArgs.Empty);
        protected virtual void OnEnteredRoom() => Entered?.Invoke(this, EventArgs.Empty);
        protected virtual void OnLeftRoom() => Left?.Invoke(this, EventArgs.Empty);

        protected virtual void OnRoomDataUpdated(RoomData roomData)
            => RoomDataUpdated?.Invoke(this, new RoomDataEventArgs(roomData));
        #endregion

        protected override void OnInitialize() { }

        private void LeaveRoom()
        {
            IsRingingDoorbell =
            IsInQueue =
            IsEnteringRoom =
            IsInRoom = false;
            QueuePosition = 0;

            Model = null;
            Data = null;
            
            DoorTile = null;
            FloorPlan = null;
            HeightMap = null;

            RightsLevel = 0;
            IsOwner = false;

            OnLeftRoom();
        }

        [Receive("RoomData")]
        private void HandleRoomData(Packet packet)
        {
            var roomData = RoomData.Parse(packet);
            roomDataCache[roomData.Id] = roomData;

            if (IsInRoom && roomData.Id == Id)
            {
                DebugUtil.Log("room data updated");

                Data = roomData;
                OnRoomDataUpdated(roomData);
            }
        }

        [Receive("RoomOpen")]
        private void HandleRoomOpen(Packet packet)
        {
            if (IsRingingDoorbell || IsInQueue || IsEnteringRoom || IsInRoom)
            {
                DebugUtil.Log($"leaving current room ({Id})");
                LeaveRoom();
            }

            Id = packet.ReadInteger();

            if (roomDataCache.ContainsKey(Id))
            {
                DebugUtil.Log("loaded room data from cache");
                Data = roomDataCache[Id];
            }
        }

        [Receive("DoorbellAddUser")]
        private void HandleDoorbellAddUser(Packet packet)
        {
            string user = packet.ReadString();
            if (string.IsNullOrEmpty(user))
            {
                DebugUtil.Log("ringing doorbell");

                IsRingingDoorbell = true;
                OnRingingDoorbell();
            }
        }

        [Receive("HideDoorbell")]
        private void HandleHideDoorbell(Packet packet)
        {
            int roomId = packet.ReadInteger();
            if (roomId != Id)
            {
                DebugUtil.Log("room id mismatch");
                return;
            }

            string user = packet.ReadString();
            if (string.IsNullOrEmpty(user))
            {
                DebugUtil.Log("no answer");
                IsRingingDoorbell = false;
            }
        }

        [Receive("RoomEntryInfo")]
        private void HandleRoomEntryInfo(Packet packet)
        {
            packet.ReadInteger(); // room id
            if (packet.ReadInteger() == 2 &&
                packet.ReadString() == "visitors" &&
                packet.ReadInteger() == 2 &&
                packet.ReadInteger() == 1 &&
                packet.ReadString() == "visitors")
            {
                bool enteredQueue = !IsInQueue;

                IsInQueue = true;
                QueuePosition = packet.ReadInteger() + 1;

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

        [Receive("RoomModel")]
        private void HandleRoomModel(Packet packet)
        {
            if (IsEnteringRoom || IsInRoom)
            {
                DebugUtil.Log($"leaving current room ({Id})");
                LeaveRoom();
            }

            IsRingingDoorbell =
            IsInQueue = false;
            QueuePosition = 0;

            Model = packet.ReadString();
            Id = packet.ReadInteger();

            IsEnteringRoom = true;

            DebugUtil.Log("entering room");
            OnEnteringRoom();
        }

        [Group(Features.Permissions), Receive("RoomRights")]
        private void HandleRoomRights(Packet packet)
        {
            int roomId = packet.ReadInteger();
            if (roomId != Id)
            {
                DebugUtil.Log("room id mismatch");
                return;
            }

            RightsLevel = packet.ReadInteger();

            DebugUtil.Log($"rights = {RightsLevel}");
        }

        [Group(Features.DoorTile), Receive("FloorPlanEditorDoorSettings")]
        private void HandleDoorSettings(Packet packet)
        {
            if (!IsEnteringRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            int x = packet.ReadInteger();
            int y = packet.ReadInteger();
            int dir = packet.ReadInteger();

            DoorTile = new Tile(x, y, 0);

            DebugUtil.Log("loaded door tile");
        }

        [Group(Features.HeightMap), Receive("RoomRelativeMap")]
        private void HandleHeightMap(Packet packet)
        {
            if (!IsEnteringRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            HeightMap = HeightMap.Parse(packet);

            DebugUtil.Log("loaded heightmap");
        }

        [Group(Features.HeightMap), Receive("UpdateStackHeight")]
        private void HandleUpdateStackHeight(Packet packet)
        {
            if (HeightMap == null) return;

            int n = packet.ReadByte();
            for (int i = 0; i < n; i++)
            {
                int x = packet.ReadByte();
                int y = packet.ReadByte();
                HeightMap[x, y] = packet.ReadShort();
            }

            DebugUtil.Log("heightmap updated");
        }

        [Group(Features.FloorPlan), Receive("RoomHeightMap")]
        private void HandleFloorPlan(Packet packet)
        {
            if (!IsEnteringRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            FloorPlan = FloorPlan.Parse(packet);

            DebugUtil.Log("loaded floor plan");
        }

        [Receive("RoomOwner")]
        private void HandleRoomOwner(Packet packet)
        {
            if (!IsEnteringRoom)
            {
                DebugUtil.Log("not entering room");
                return;
            }

            int roomId = packet.ReadInteger();
            if (roomId != Id)
            {
                DebugUtil.Log("room id mismatch");
                return;
            }

            IsOwner = packet.ReadBoolean();
            IsEnteringRoom = false;
            IsInRoom = true;

            if (roomDataCache.TryGetValue(roomId, out RoomData roomData))
            {
                DebugUtil.Log("loaded room data from cache");
                Data = roomData;
            }

            DebugUtil.Log("entered room");
            OnEnteredRoom();
        }

        [Receive("HotelView")]
        private void HandleHotelView(Packet packet)
        {
            if (IsRingingDoorbell || IsInQueue || IsEnteringRoom || IsInRoom)
            {
                DebugUtil.Log($"leaving current room ({Id})");
                LeaveRoom();
            }
        }
    }
}
