using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IModerationSettings"/>
public class ModerationSettings : IModerationSettings, IParserComposer<ModerationSettings>
{
    public ModerationPermissions Mute { get; set; }
    public ModerationPermissions Kick { get; set; }
    public ModerationPermissions Ban { get; set; }

    public ModerationSettings() { }

    internal ModerationSettings(in PacketReader p)
    {
        Mute = (ModerationPermissions)p.ReadInt();
        Kick = (ModerationPermissions)p.ReadInt();
        Ban = (ModerationPermissions)p.ReadInt();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt((int)Mute);
        p.WriteInt((int)Kick);
        p.WriteInt((int)Ban);
    }

    static ModerationSettings IParser<ModerationSettings>.Parse(in PacketReader p) => new(in p);
}
