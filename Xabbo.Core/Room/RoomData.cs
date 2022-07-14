using System;

using Xabbo.Common;
using Xabbo.Messages;

namespace Xabbo.Core;

public class RoomData : RoomInfo, IRoomData
{
    public bool IsEntering { get; set; }
    public bool Forward { get; set; }
    public bool IsStaffPick { get; set; }
    public bool IsGroupMember { get; set; }
    public bool IsRoomMuted { get; set; }
    public ModerationSettings Moderation { get; set; }
    IModerationSettings IRoomData.Moderation => Moderation;
    public bool CanMute { get; set; }
    public ChatSettings ChatSettings { get; set; }
    IChatSettings IRoomData.ChatSettings => ChatSettings;

    public int _Int6 { get; set; }
    public int _Int7 { get; set; }

    public RoomData()
    {
        Moderation = new ModerationSettings();
        ChatSettings = new ChatSettings();
    }

    protected RoomData(bool isEntering, IReadOnlyPacket packet)
        : base(packet)
    {
        IsEntering = isEntering;

        Forward = packet.ReadBool();
        IsStaffPick = packet.ReadBool();
        IsGroupMember = packet.ReadBool();
        IsRoomMuted = packet.ReadBool();

        Moderation = ModerationSettings.Parse(packet);

        CanMute = packet.ReadBool();
        ChatSettings = ChatSettings.Parse(packet);

        if (packet.Protocol == ClientType.Unity)
        {
            _Int6 = packet.ReadInt();
            _Int7 = packet.ReadInt();
        }
    }

    public override void Compose(IPacket packet)
    {
        packet.WriteBool(IsEntering);

        base.Compose(packet);

        packet.WriteBool(Forward);
        packet.WriteBool(IsStaffPick);
        packet.WriteBool(IsGroupMember);
        packet.WriteBool(IsRoomMuted);

        Moderation.Compose(packet);

        packet.WriteBool(CanMute);

        ChatSettings.Compose(packet);
    }

    public static new RoomData Parse(IReadOnlyPacket packet)
    {
        return new RoomData(packet.ReadBool(), packet);
    }
}
