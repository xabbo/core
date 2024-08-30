namespace Xabbo.Core;

public interface IRoomData : IRoomInfo
{
    bool IsEntering { get; }
    bool Forward { get; }
    bool IsGroupMember { get; }
    bool IsRoomMuted { get; }
    IModerationSettings Moderation { get; }
    bool CanMute { get; }
    IChatSettings ChatSettings { get; }
}
