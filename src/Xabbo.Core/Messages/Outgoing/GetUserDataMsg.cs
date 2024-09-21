using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when requesting the user's data.
/// <para/>
/// Request for <see cref="UserDataMsg"/>.
/// </summary>
public sealed record GetUserDataMsg : IRequestMessage<GetUserDataMsg, UserDataMsg, UserData>
{
    static Identifier IMessage<GetUserDataMsg>.Identifier => Out.InfoRetrieve;
    UserData IResponseData<UserDataMsg, UserData>.GetData(UserDataMsg msg) => msg.UserData;
    static GetUserDataMsg IParser<GetUserDataMsg>.Parse(in PacketReader p) => new();
    void IComposer.Compose(in PacketWriter p) { }
}
