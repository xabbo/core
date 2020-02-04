using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public interface IGameState
    {
        #region - User data -
        UserData UserData { get; }
        int UserId { get; }
        string UserName { get; }
        Gender UserGender { get; }
        string UserFigure { get; }
        string UserMotto { get; }
        bool IsNameChangeable { get; }

        int HomeRoomId { get; }
        #endregion

        #region - Inventory -
        int Credits { get; }
        int Diamonds { get; }
        int Duckets { get; }
        #endregion

        #region - Room state -
        RoomData RoomData { get; }
        int RoomId { get; }
        bool IsRingingDoorbell { get; }
        bool IsInQueue { get; }
        int QueuePosition { get; }
        bool IsLoadingRoom { get; }
        bool IsInRoom { get; }

        Tile DoorTile { get; }
        HeightMap HeightMap { get; }
        FloorPlan FloorPlan { get; }
        #endregion

        #region - Room permissions -
        bool IsRoomOwner { get; }
        bool HasRights { get; }
        int RightsLevel { get; }
        bool CanMute { get; }
        bool CanKick { get; }
        bool CanBan { get; }
        #endregion

        #region - Furni -
        IEnumerable<Furni> Furni { get; }
        IEnumerable<FloorItem> FloorItems { get; }
        IEnumerable<WallItem> WallItems { get; }
        FloorItem GetFloorItem(int id);
        WallItem GetWallItem(int id);
        #endregion

        #region - Entities -
        IEnumerable<Entity> Entities { get; }
        T GetEntity<T>(int index) where T : Entity;
        T GetEntity<T>(string name) where T : Entity;
        T GetEntityById<T>(int id) where T : Entity;
        #endregion

        #region - Chat state -
        bool IsMuted { get; }
        bool IsFlooded { get; }
        DateTime MuteEnd { get; }
        #endregion

        /*#region - Events -
        event EventHandler LoadedUserData;
        // Room
        event EventHandler RingingDoorbell;
        event EventHandler DoorbellUnanswered;
        event EventHandler EnteredQueue;
        event EventHandler QueueStatusUpdated;
        event EventHandler EnteringRoom;
        event EventHandler EnteredRoom;
        event EventHandler LeftRoom;
        #endregion*/
    }
}
