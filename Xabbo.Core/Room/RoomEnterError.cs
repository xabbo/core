using System;

namespace Xabbo.Core;

public enum RoomEnterError
{
    Full = 1,
    CannotEnter = 2,
    // 3 client disconnects
    Banned = 4,
    // The following are not used in the RoomEnterError packet
    Unknown = -1,
    WrongPassword = -100002
}
