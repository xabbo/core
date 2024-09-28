using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Response to <see cref="Incoming.PingMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.Pong"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.PONG"/></item>
/// </list>
/// </summary>
public sealed record PongMsg : IMessage<PongMsg>
{
    static Identifier IMessage<PongMsg>.Identifier => Out.Pong;
    static PongMsg IParser<PongMsg>.Parse(in PacketReader p) => new();
    void IComposer.Compose(in PacketWriter p) { }
}