using System;

namespace Xabbo.Core;

public enum RoomAccess
{
    None = -1,
    Open = 0,
    Doorbell = 1,
    Password = 2,
    Invisible = 3,
    Friends = 7
}
