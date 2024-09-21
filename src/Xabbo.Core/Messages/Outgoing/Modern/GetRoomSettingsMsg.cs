using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Request for <see cref="RoomSettingsMsg"/>.
/// </summary>
public sealed record GetRoomSettingsMsg(Id Id) : IRequestMessage<GetRoomSettingsMsg, RoomSettingsMsg, RoomSettings>
{
    static Identifier IMessage<GetRoomSettingsMsg>.Identifier => Out.GetRoomSettings;
    bool IRequestFor<RoomSettingsMsg>.MatchResponse(RoomSettingsMsg msg) => msg.Settings.Id == Id;
    RoomSettings IResponseData<RoomSettingsMsg, RoomSettings>.GetData(RoomSettingsMsg msg) => msg.Settings;
    static GetRoomSettingsMsg IParser<GetRoomSettingsMsg>.Parse(in PacketReader p) => new(p.ReadId());
    void IComposer.Compose(in PacketWriter p) => p.WriteId(Id);
}
