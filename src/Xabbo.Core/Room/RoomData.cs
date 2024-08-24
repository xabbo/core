using Xabbo.Messages;

namespace Xabbo.Core;

public class RoomData : RoomInfo, IRoomData, IComposer, IParser<RoomData>
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

    protected RoomData(bool isEntering, in PacketReader p) : base(in p)
    {
        IsEntering = isEntering;

        Forward = p.Read<bool>();
        IsStaffPick = p.Read<bool>();
        IsGroupMember = p.Read<bool>();
        IsRoomMuted = p.Read<bool>();

        Moderation = ModerationSettings.Parse(p);

        CanMute = p.Read<bool>();
        ChatSettings = ChatSettings.Parse(p);

        if (p.Client == ClientType.Unity)
        {
            _Int6 = p.Read<int>();
            _Int7 = p.Read<int>();
        }
    }

    public override void Compose(in PacketWriter p)
    {
        p.Write(IsEntering);

        base.Compose(in p);

        p.Write(Forward);
        p.Write(IsStaffPick);
        p.Write(IsGroupMember);
        p.Write(IsRoomMuted);

        p.Write(Moderation);

        p.Write(CanMute);

        ChatSettings.Compose(p);
    }

    public static new RoomData Parse(in PacketReader packet) => new(packet.Read<bool>(), in packet);
}
