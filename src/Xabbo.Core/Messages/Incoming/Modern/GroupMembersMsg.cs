using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Response for <see cref="Outgoing.Modern.GetGroupMembersMsg"/>.
/// </summary>
public sealed record GroupMembersMsg(GroupMembers Members) : IMessage<GroupMembersMsg>
{
    static Identifier IMessage<GroupMembersMsg>.Identifier => In.GuildMembers;
    static GroupMembersMsg IParser<GroupMembersMsg>.Parse(in PacketReader p) => new(p.Parse<GroupMembers>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Members);
}
