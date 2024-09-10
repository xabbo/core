using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record PingMsg : IMessage<PingMsg>
{
    static Identifier IMessage<PingMsg>.Identifier => In.Ping;
    static PingMsg IParser<PingMsg>.Parse(in PacketReader p) => new();
    void IComposer.Compose(in PacketWriter p) { }
}