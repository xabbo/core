using Xabbo.Messages;

namespace Xabbo.Core;

public class RoomData : RoomInfo, IRoomData, IParserComposer<RoomData>
{
    public bool IsEntering { get; set; }
    public bool Forward { get; set; }
    public bool IsStaffPick { get; set; }
    public bool IsGroupMember { get; set; }
    public bool IsRoomMuted { get; set; }
    public ModerationSettings Moderation { get; set; } = new();
    IModerationSettings IRoomData.Moderation => Moderation;
    public bool CanMute { get; set; }
    public ChatSettings ChatSettings { get; set; } = new();
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
        if (p.Client == ClientType.Shockwave)
            return;

        IsEntering = isEntering;

        Forward = p.ReadBool();
        IsStaffPick = p.ReadBool();
        IsGroupMember = p.ReadBool();
        IsRoomMuted = p.ReadBool();

        Moderation = p.Parse<ModerationSettings>();

        CanMute = p.ReadBool();
        ChatSettings = p.Parse<ChatSettings>();

        if (p.Client == ClientType.Unity)
        {
            _Int6 = p.ReadInt();
            _Int7 = p.ReadInt();
        }
    }

    protected override void Compose(in PacketWriter p)
    {
        p.WriteBool(IsEntering);

        base.Compose(in p);

        p.WriteBool(Forward);
        p.WriteBool(IsStaffPick);
        p.WriteBool(IsGroupMember);
        p.WriteBool(IsRoomMuted);

        p.Compose(Moderation);

        p.WriteBool(CanMute);

        p.Compose(ChatSettings);
    }

    static RoomData IParser<RoomData>.Parse(in PacketReader p)
    {
        bool isEntering = false;
        if (p.Client != ClientType.Shockwave)
            isEntering = p.ReadBool();
        return new(isEntering, in p);
    }
}
