using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received after requesting the user's data.
/// <para/>
/// Response for <see cref="Outgoing.GetUserDataMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.UserObject"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.USER_OBJ"/></item>
/// </list>
/// </summary>
/// <param name="UserData">The current user's data.</param>
public sealed record UserDataMsg(UserData UserData) : IMessage<UserDataMsg>
{
    static Identifier IMessage<UserDataMsg>.Identifier => In.UserObject;
    static UserDataMsg IParser<UserDataMsg>.Parse(in PacketReader p) => new(p.Parse<UserData>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(UserData);
}
