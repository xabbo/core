using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Response for <see cref="Outgoing.GetUserDataMsg"/>.
/// </summary>
public sealed record UserDataMsg(UserData UserData) : IMessage<UserDataMsg>
{
    static Identifier IMessage<UserDataMsg>.Identifier => In.UserObject;
    static UserDataMsg IParser<UserDataMsg>.Parse(in PacketReader p) => new(p.Parse<UserData>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(UserData);
}
