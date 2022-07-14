using System;

namespace Xabbo.Core;

[Flags]
public enum RoomFlags
{
    None = 0,
    HasOfficialRoomPic = 1,
    IsGroupHomeRoom = 2,
    HasEvent = 4,
    ShowOwnerName = 8,
    AllowPets = 16,
    ShowRoomAd = 32
}
