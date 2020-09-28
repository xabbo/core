using System;

namespace Xabbo.Core
{
    public interface IRoomData : IRoomInfo
    {
        bool IsUpdating { get; }
        bool ForceLoad { get; }
        bool IsRoomMuted { get; }
        IModerationSettings Moderation { get; }
        bool ShowMuteButton { get; }
        IChatSettings ChatSettings { get; }
    }
}
