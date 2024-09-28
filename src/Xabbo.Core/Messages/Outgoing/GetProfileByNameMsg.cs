using System;
using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when requesting a user's profile.
/// <para/>
/// Request for <see cref="ProfileMsg"/>. Returns <see cref="UserProfile"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.GetExtendedProfileByName"/></item>
/// </list>
/// </summary>
/// <param name="Name">The name of the user whose profile to request.</param>
public sealed record GetProfileByNameMsg(string Name) : IRequestMessage<GetProfileByNameMsg, ProfileMsg, UserProfile>
{
    static ClientType IMessage<GetProfileByNameMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GetProfileByNameMsg>.Identifier => Out.GetExtendedProfileByName;
    bool IRequestFor<ProfileMsg>.MatchResponse(ProfileMsg response)
        => response.Profile.Name.Equals(Name, StringComparison.OrdinalIgnoreCase);
    UserProfile IResponseData<ProfileMsg, UserProfile>.GetData(ProfileMsg msg) => msg.Profile;
    static GetProfileByNameMsg IParser<GetProfileByNameMsg>.Parse(in PacketReader p) => new(p.ReadString());
    void IComposer.Compose(in PacketWriter p) => p.WriteString(Name);
}
