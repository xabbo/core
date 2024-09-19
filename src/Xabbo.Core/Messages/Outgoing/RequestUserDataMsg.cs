using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record RequestUserDataMsg : IMessage<RequestUserDataMsg>
{
    public static Identifier Identifier => Out.InfoRetrieve;
    public static RequestUserDataMsg Parse(in PacketReader p) => new();
    public void Compose(in PacketWriter p) { }
}
