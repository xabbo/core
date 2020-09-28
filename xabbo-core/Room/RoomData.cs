using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class RoomData : RoomInfo, IRoomData
    {
        public static new RoomData Parse(Packet packet) => new RoomData(packet.ReadBool(), packet);

        public bool IsUpdating { get; set; }
        public bool ForceLoad { get; set; }
        public bool Bool3 { get; set; }
        public bool Bool4 { get; set; }
        public bool IsRoomMuted { get; set; }
        public ModerationSettings Moderation { get; set; }
        IModerationSettings IRoomData.Moderation => Moderation;
        public bool ShowMuteButton { get; set; }
        public ChatSettings ChatSettings { get; set; }
        IChatSettings IRoomData.ChatSettings => ChatSettings;

        public RoomData() { }

        internal RoomData(bool isUpdating, Packet packet)
            : base(packet)
        {
            IsUpdating = isUpdating;

            ForceLoad = packet.ReadBool(); // if IsUpdating == false
            Bool3 = packet.ReadBool();
            Bool4 = packet.ReadBool();
            IsRoomMuted = packet.ReadBool();

            Moderation = ModerationSettings.Parse(packet);

            ShowMuteButton = packet.ReadBool();
            ChatSettings = ChatSettings.Parse(packet);
        }
    }
}
