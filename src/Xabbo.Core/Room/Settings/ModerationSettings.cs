using Xabbo.Messages;

namespace Xabbo.Core;

public class ModerationSettings : IModerationSettings, IComposer, IParser<ModerationSettings>
{
    public ModerationPermissions WhoCanMute { get; set; }
    public ModerationPermissions WhoCanKick { get; set; }
    public ModerationPermissions WhoCanBan { get; set; }

    public ModerationSettings() { }

    internal ModerationSettings(in PacketReader p)
    {
        WhoCanMute = (ModerationPermissions)p.Read<int>();
        WhoCanKick = (ModerationPermissions)p.Read<int>();
        WhoCanBan = (ModerationPermissions)p.Read<int>();
    }

    public void Compose(in PacketWriter p)
    {
        p.Write((int)WhoCanMute);
        p.Write((int)WhoCanKick);
        p.Write((int)WhoCanBan);
    }

    public static ModerationSettings Parse(in PacketReader p) => new(in p);
}
