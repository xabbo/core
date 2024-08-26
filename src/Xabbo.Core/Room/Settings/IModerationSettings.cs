using Xabbo.Messages;

namespace Xabbo.Core;

public interface IModerationSettings : IComposer
{
    ModerationPermissions WhoCanMute { get; }
    ModerationPermissions WhoCanKick { get; }
    ModerationPermissions WhoCanBan { get; }
}
