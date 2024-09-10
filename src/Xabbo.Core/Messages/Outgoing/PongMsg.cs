using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

public sealed record PongMsg : IMessage<PongMsg>
{
    static Identifier IMessage<PongMsg>.Identifier => Out.Pong;
    static PongMsg IParser<PongMsg>.Parse(in PacketReader p) => new();
    void IComposer.Compose(in PacketWriter p) { }
}