using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when requesting the settings for a room.
/// <para/>
/// Request for <see cref="RoomSettingsMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.GetRoomSettings"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the room to request settings for.</param>
public sealed record GetRoomSettingsMsg(Id Id) : IRequestMessage<GetRoomSettingsMsg, RoomSettingsMsg, RoomSettings>
{
    static ClientType IMessage<GetRoomSettingsMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GetRoomSettingsMsg>.Identifier => Out.GetRoomSettings;
    bool IRequestFor<RoomSettingsMsg>.MatchResponse(RoomSettingsMsg msg) => msg.Settings.Id == Id;
    RoomSettings IResponseData<RoomSettingsMsg, RoomSettings>.GetData(RoomSettingsMsg msg) => msg.Settings;
    static GetRoomSettingsMsg IParser<GetRoomSettingsMsg>.Parse(in PacketReader p) => new(p.ReadId());
    void IComposer.Compose(in PacketWriter p) => p.WriteId(Id);
}
