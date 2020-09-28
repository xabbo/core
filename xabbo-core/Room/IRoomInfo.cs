using System;
using System.Collections.Generic;

namespace Xabbo.Core
{
    public interface IRoomInfo
    {
        int Id { get; }
        string Name { get; }
        int OwnerId { get; }
        string OwnerName { get; }
        RoomAccess Access { get; }
        bool IsOpen { get; }
        bool IsDoorbell { get; }
        bool IsLocked { get; }
        bool IsInvisible { get; }
        int Users { get; }
        int MaxUsers { get; }
        string Description { get; }
        TradePermissions Trading { get; }
        int Score { get; }
        int Ranking { get; }
        RoomCategory Category { get; }
        IReadOnlyList<string> Tags { get; }

        RoomFlags Flags { get; }
        bool HasOfficialRoomPic { get; }
        bool IsGroupHomeRoom { get; }
        bool HasEvent { get; }
        bool ShowOwnerName { get; }
        bool AllowPets { get; }

        string OfficialRoomPicRef { get; }
        int GroupId { get; }
        string GroupName { get; }
        string GroupBadge { get; }
        string EventName { get; }
        string EventDescription { get; }
        int EventMinutesLeft { get; }
    }
}
