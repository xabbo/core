using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received periodically to check if the connection is alive.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.Ping"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.PING"/></item>
/// </list>
/// </summary>
public sealed record PingMsg : IMessage<PingMsg>
{
    static Identifier IMessage<PingMsg>.Identifier => In.Ping;
    static PingMsg IParser<PingMsg>.Parse(in PacketReader p) => new();
    void IComposer.Compose(in PacketWriter p) { }
}
