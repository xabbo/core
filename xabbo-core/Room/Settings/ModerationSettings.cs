using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class ModerationSettings : IModerationSettings, IWritable
    {
        public static ModerationSettings Parse(Packet packet) => new ModerationSettings(packet);

        public ModerationPermissions WhoCanMute { get; set; }
        public ModerationPermissions WhoCanKick { get; set; }
        public ModerationPermissions WhoCanBan { get; set; }

        public ModerationSettings() { }

        internal ModerationSettings(Packet packet)
        {
            WhoCanMute = (ModerationPermissions)packet.ReadInt();
            WhoCanKick = (ModerationPermissions)packet.ReadInt();
            WhoCanBan = (ModerationPermissions)packet.ReadInt();
        }

        public void Write(Packet packet) => packet.WriteValues(
            (int)WhoCanMute,
            (int)WhoCanKick,
            (int)WhoCanBan
        );
    }
}
