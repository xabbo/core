using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class RoomData : RoomInfo
    {
        public static new RoomData Parse(Packet packet) => new RoomData(packet.ReadBoolean(), packet);

        public bool IsUpdating { get; set; }
        public bool ForceLoad { get; set; }
        public bool UnknownBoolA { get; set; }
        public bool UnknownBoolB { get; set; }
        public bool IsRoomMuted { get; set; }
        public ModerationSettings Moderation { get; set; }
        public bool ShowMuteButton { get; set; }
        public ChatSettings ChatSettings { get; set; }

        public RoomData() { }

        internal RoomData(bool isUpdating, Packet packet)
            : base(packet)
        {
            IsUpdating = isUpdating;

            ForceLoad = packet.ReadBoolean(); // if IsUpdating == false
            UnknownBoolA = packet.ReadBoolean();
            UnknownBoolB = packet.ReadBoolean();
            IsRoomMuted = packet.ReadBoolean();

            Moderation = ModerationSettings.Parse(packet);

            ShowMuteButton = packet.ReadBoolean();
            ChatSettings = ChatSettings.Parse(packet);
        }
    }
}
