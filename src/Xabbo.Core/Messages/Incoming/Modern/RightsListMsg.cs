using System.Collections.Generic;
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Response for <see cref="Outgoing.Modern.GetRightsListMsg"/>.
/// </summary>
public sealed record RightsListMsg(Id RoomId, List<IdName> Users) : IMessage<RightsListMsg>
{
    static ClientType IMessage<RightsListMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<RightsListMsg>.Identifier => In.FlatControllers;
    static RightsListMsg IParser<RightsListMsg>.Parse(in PacketReader p) => new(
        RoomId: p.ReadId(),
        Users: [.. p.ParseArray<IdName>()]
    );
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(RoomId);
        p.ComposeArray(Users);
    }
}
