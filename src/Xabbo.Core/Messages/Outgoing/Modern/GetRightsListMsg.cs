using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Request for <see cref="RightsListMsg"/>.
/// </summary>
public sealed record GetRightsListMsg(Id RoomId) : IRequestMessage<GetRightsListMsg, RightsListMsg>
{
    static ClientType IMessage<GetRightsListMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GetRightsListMsg>.Identifier => Out.GetFlatControllers;
    bool IRequestFor<RightsListMsg>.MatchResponse(RightsListMsg msg) => msg.RoomId == RoomId;
    static GetRightsListMsg IParser<GetRightsListMsg>.Parse(in PacketReader p) => new(p.ReadId());
    void IComposer.Compose(in PacketWriter p) => p.WriteId(RoomId);
}
