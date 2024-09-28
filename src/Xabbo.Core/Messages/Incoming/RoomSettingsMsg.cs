using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received after requesting the settings of a room.
/// <para/>
/// Response for <see cref="Outgoing.GetRoomSettingsMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.RoomSettingsData"/></item>
/// </list>
/// </summary>
/// <param name="Settings">The room settings.</param>
public sealed record RoomSettingsMsg(RoomSettings Settings) : IMessage<RoomSettingsMsg>
{
    static ClientType IMessage<RoomSettingsMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<RoomSettingsMsg>.Identifier => In.RoomSettingsData;
    static RoomSettingsMsg IParser<RoomSettingsMsg>.Parse(in PacketReader p) => new(p.Parse<RoomSettings>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Settings);
}
