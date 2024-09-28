using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when requesting the rights list of a room.
/// <para/>
/// Request for <see cref="RightsListMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.GetFlatControllers"/></item>
/// </list>
/// </summary>
/// <param name="RoomId">The ID of the room to request the rights list for.</param>
public sealed record GetRightsListMsg(Id RoomId) : IRequestMessage<GetRightsListMsg, RightsListMsg>
{
    static ClientType IMessage<GetRightsListMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GetRightsListMsg>.Identifier => Out.GetFlatControllers;
    bool IRequestFor<RightsListMsg>.MatchResponse(RightsListMsg msg) => msg.RoomId == RoomId;
    static GetRightsListMsg IParser<GetRightsListMsg>.Parse(in PacketReader p) => new(p.ReadId());
    void IComposer.Compose(in PacketWriter p) => p.WriteId(RoomId);
}
