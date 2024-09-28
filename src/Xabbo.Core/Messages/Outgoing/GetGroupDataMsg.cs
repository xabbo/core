using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when requesting a group's information.
/// <para/>
/// Request for <see cref="GroupDataMsg"/>. Returns <see cref="GroupData"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.GetHabboGroupDetails"/></item>
/// </list>
/// </summary>
/// <param name="Id">The ID of the group to request information for.</param>
/// <param name="Open">Whether to open the group information in-client.</param>
public sealed record GetGroupDataMsg(Id Id, bool Open = false) : IRequestMessage<GetGroupDataMsg, GroupDataMsg, GroupData>
{
    static ClientType IMessage<GetGroupDataMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GetGroupDataMsg>.Identifier => Out.GetHabboGroupDetails;
    bool IRequestFor<GroupDataMsg>.MatchResponse(GroupDataMsg msg) => msg.Group.Id == Id;
    GroupData IResponseData<GroupDataMsg, GroupData>.GetData(GroupDataMsg msg) => msg.Group;
    static GetGroupDataMsg IParser<GetGroupDataMsg>.Parse(in PacketReader p) => new(p.ReadId(), p.ReadBool());
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(Id);
        p.WriteBool(Open);
    }
}
