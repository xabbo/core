using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received after requesting a user's profile.
/// <para/>
/// Response for <see cref="Outgoing.GetProfileMsg"/> or <see cref="Outgoing.GetProfileByNameMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.ExtendedProfile"/></item>
/// </list>
/// </summary>
/// <param name="Profile">The requested user's profile.</param>
public sealed record ProfileMsg(UserProfile Profile) : IMessage<ProfileMsg>
{
    static ClientType IMessage<ProfileMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<ProfileMsg>.Identifier => In.ExtendedProfile;
    static ProfileMsg IParser<ProfileMsg>.Parse(in PacketReader p) => new(p.Parse<UserProfile>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Profile);
}
