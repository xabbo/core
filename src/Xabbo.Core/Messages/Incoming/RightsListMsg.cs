using System.Collections.Generic;
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received after requesting the room rights list.
/// <para/>
/// Response for <see cref="Outgoing.GetRightsListMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.FlatControllers"/></item>
/// </list>
/// </summary>
/// <param name="RoomId">The ID of the room.</param>
/// <param name="Users">The list of users with rights.</param>
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
