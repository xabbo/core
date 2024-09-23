using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Response for <see cref="Outgoing.Modern.GetGroupDataMsg"/>.
/// </summary>
public sealed record GroupDataMsg(GroupData Group) : IMessage<GroupDataMsg>
{
    static ClientType IMessage<GroupDataMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GroupDataMsg>.Identifier => In.HabboGroupDetails;
    static GroupDataMsg IParser<GroupDataMsg>.Parse(in PacketReader p) => new(p.Parse<GroupData>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Group);
}
