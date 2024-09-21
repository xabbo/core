using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Sent when requesting a group's information.
/// <para/>
/// Request for <see cref="GroupDataMsg"/>.
/// </summary>
/// <param name="Id">The ID of the group to request information for.</param>
/// <param name="Open">Whether the open the group information in-client.</param>
public sealed record GetGroupDataMsg(Id Id, bool Open = false) : IRequestMessage<GetGroupDataMsg, GroupDataMsg>
{
    static Identifier IMessage<GetGroupDataMsg>.Identifier => Out.GetHabboGroupDetails;
    bool IRequestFor<GroupDataMsg>.MatchResponse(GroupDataMsg msg) => msg.Group.Id == Id;
    static GetGroupDataMsg IParser<GetGroupDataMsg>.Parse(in PacketReader p) => new(p.ReadId(), p.ReadBool());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteBool(Open);
    }
}
