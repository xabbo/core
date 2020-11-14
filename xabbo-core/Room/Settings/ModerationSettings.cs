using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class ModerationSettings : IModerationSettings
    {
        public static ModerationSettings Parse(IReadOnlyPacket packet) => new ModerationSettings(packet);

        public ModerationPermissions WhoCanMute { get; set; }
        public ModerationPermissions WhoCanKick { get; set; }
        public ModerationPermissions WhoCanBan { get; set; }

        public ModerationSettings() { }

        internal ModerationSettings(IReadOnlyPacket packet)
        {
            WhoCanMute = (ModerationPermissions)packet.ReadInt();
            WhoCanKick = (ModerationPermissions)packet.ReadInt();
            WhoCanBan = (ModerationPermissions)packet.ReadInt();
        }

        public void Write(IPacket packet) => packet.WriteValues(
            (int)WhoCanMute,
            (int)WhoCanKick,
            (int)WhoCanBan
        );
    }
}
