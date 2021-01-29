using System;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class RoomData : RoomInfo, IRoomData
    {
        public static new RoomData Parse(IReadOnlyPacket packet) => new RoomData(packet.ReadBool(), packet);

        public bool IsUpdating { get; set; }
        public bool ForceLoad { get; set; }
        public bool Bool3 { get; set; }
        public bool BypassAccess { get; set; }
        public bool IsRoomMuted { get; set; }
        public ModerationSettings Moderation { get; set; }
        IModerationSettings IRoomData.Moderation => Moderation;
        public bool ShowMuteButton { get; set; }
        public ChatSettings ChatSettings { get; set; }
        IChatSettings IRoomData.ChatSettings => ChatSettings;

        public int UnknownInt1 { get; set; }
        public int UnknownInt2 { get; set; }

        public RoomData() { }

        protected RoomData(bool isUpdating, IReadOnlyPacket packet)
            : base(packet)
        {
            IsUpdating = isUpdating;

            ForceLoad = packet.ReadBool(); // if IsUpdating == false
            Bool3 = packet.ReadBool();
            BypassAccess = packet.ReadBool();
            IsRoomMuted = packet.ReadBool();

            Moderation = ModerationSettings.Parse(packet);

            ShowMuteButton = packet.ReadBool();
            ChatSettings = ChatSettings.Parse(packet);

            UnknownInt1 = packet.ReadInt();
            UnknownInt2 = packet.ReadInt();
        }

        public override void Write(IPacket packet)
        {
            packet.WriteBool(IsUpdating);

            base.Write(packet);

            packet.WriteBool(ForceLoad);
            packet.WriteBool(Bool3);
            packet.WriteBool(BypassAccess);
            packet.WriteBool(IsRoomMuted);

            Moderation.Write(packet);

            packet.WriteBool(ShowMuteButton);

            ChatSettings.Write(packet);
        }
    }
}
