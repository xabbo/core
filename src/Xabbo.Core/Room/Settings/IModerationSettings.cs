namespace Xabbo.Core;

public interface IModerationSettings
{
    ModerationPermissions WhoCanMute { get; }
    ModerationPermissions WhoCanKick { get; }
    ModerationPermissions WhoCanBan { get; }
}
