using System;
using Xabbo.Messages;

namespace Xabbo.Core;

public interface IModerationSettings : IComposable
{
    ModerationPermissions WhoCanMute { get; }
    ModerationPermissions WhoCanKick { get; }
    ModerationPermissions WhoCanBan { get; }
}
