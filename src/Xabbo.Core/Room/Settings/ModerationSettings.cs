using Xabbo.Messages;

namespace Xabbo.Core;

public class ModerationSettings : IModerationSettings, IParserComposer<ModerationSettings>
{
    public ModerationPermissions WhoCanMute { get; set; }
    public ModerationPermissions WhoCanKick { get; set; }
    public ModerationPermissions WhoCanBan { get; set; }

    public ModerationSettings() { }

    internal ModerationSettings(in PacketReader p)
    {
        WhoCanMute = (ModerationPermissions)p.ReadInt();
        WhoCanKick = (ModerationPermissions)p.ReadInt();
        WhoCanBan = (ModerationPermissions)p.ReadInt();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt((int)WhoCanMute);
        p.WriteInt((int)WhoCanKick);
        p.WriteInt((int)WhoCanBan);
    }

    static ModerationSettings IParser<ModerationSettings>.Parse(in PacketReader p) => new(in p);
}
