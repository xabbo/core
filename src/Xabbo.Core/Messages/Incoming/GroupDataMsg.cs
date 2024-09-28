using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received after requesting a group's information.
/// <para/>
/// Response for <see cref="Outgoing.GetGroupDataMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.HabboGroupDetails"/></item>
/// </list>
/// </summary>
/// <param name="Group">The group's information.</param>
public sealed record GroupDataMsg(GroupData Group) : IMessage<GroupDataMsg>
{
    static ClientType IMessage<GroupDataMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GroupDataMsg>.Identifier => In.HabboGroupDetails;
    static GroupDataMsg IParser<GroupDataMsg>.Parse(in PacketReader p) => new(p.Parse<GroupData>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Group);
}
