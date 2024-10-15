using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Shockwave;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when multiple console messages are received from friends.
/// <para/>
/// Supported clients: <see cref="ClientType.Origins"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.MESSENGER_MESSAGES"/></item>
/// </list>
/// </summary>
public sealed class ConsoleMessagesMsg : List<ConsoleMessage>, IMessage<ConsoleMessagesMsg>
{
    static ClientType IMessage<ConsoleMessagesMsg>.SupportedClients => ClientType.Origins;
    static Identifier IMessage<ConsoleMessagesMsg>.Identifier => In.MESSENGER_MESSAGES;
    static ConsoleMessagesMsg IParser<ConsoleMessagesMsg>.Parse(in PacketReader p) => [.. p.ParseArray<ConsoleMessage>()];
    void IComposer.Compose(in PacketWriter p) => p.ComposeArray(this);
}
