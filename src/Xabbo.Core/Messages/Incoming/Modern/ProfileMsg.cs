using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Response for <see cref="Outgoing.Modern.GetProfileMsg"/> or <see cref="Outgoing.Modern.GetProfileByNameMsg"/> .
/// </summary>
public sealed record ProfileMsg(UserProfile Profile) : IMessage<ProfileMsg>
{
    static ClientType IMessage<ProfileMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<ProfileMsg>.Identifier => In.ExtendedProfile;
    static ProfileMsg IParser<ProfileMsg>.Parse(in PacketReader p) => new(p.Parse<UserProfile>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Profile);
}
