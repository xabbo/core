using System;

using Xabbo.Core.Messages;

namespace Xabbo.Core
{
    public interface IModerationSettings : IPacketData
    {
        ModerationPermissions WhoCanMute { get; }
        ModerationPermissions WhoCanKick { get; }
        ModerationPermissions WhoCanBan { get; }
    }
}
