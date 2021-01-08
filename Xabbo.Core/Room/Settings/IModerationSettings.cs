using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public interface IModerationSettings : IPacketData
    {
        ModerationPermissions WhoCanMute { get; }
        ModerationPermissions WhoCanKick { get; }
        ModerationPermissions WhoCanBan { get; }
    }
}
