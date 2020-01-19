using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class ModerationSettings : IWritable
    {
        public static ModerationSettings Parse(Packet packet) => new ModerationSettings(packet);

        public ModerationPermissions WhoCanMute { get; internal set; }
        public ModerationPermissions WhoCanKick { get; internal set; }
        public ModerationPermissions WhoCanBan { get; internal set; }

        public ModerationSettings() { }

        internal ModerationSettings(Packet packet)
        {
            WhoCanMute = (ModerationPermissions)packet.ReadInteger();
            WhoCanKick = (ModerationPermissions)packet.ReadInteger();
            WhoCanBan = (ModerationPermissions)packet.ReadInteger();
        }

        public void Write(Packet packet) => packet.WriteValues(
            (int)WhoCanMute,
            (int)WhoCanKick,
            (int)WhoCanBan
        );
    }
}
