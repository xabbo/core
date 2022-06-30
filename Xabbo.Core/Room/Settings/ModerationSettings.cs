using System;

using Xabbo.Messages;

namespace Xabbo.Core
{
    public class ModerationSettings : IModerationSettings
    {
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

        public void Compose(IPacket packet) => packet
            .WriteInt((int)WhoCanMute)
            .WriteInt((int)WhoCanKick)
            .WriteInt((int)WhoCanBan);

        public static ModerationSettings Parse(IReadOnlyPacket packet)
        {
            return new ModerationSettings(packet);
        }
    }
}
