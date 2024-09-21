using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Response for <see cref="Outgoing.Modern.GetRoomSettingsMsg"/>.
/// </summary>
public sealed record RoomSettingsMsg(RoomSettings Settings) : IMessage<RoomSettingsMsg>
{
    static Identifier IMessage<RoomSettingsMsg>.Identifier => In.RoomSettingsData;
    static RoomSettingsMsg IParser<RoomSettingsMsg>.Parse(in PacketReader p) => new(p.Parse<RoomSettings>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Settings);
}
