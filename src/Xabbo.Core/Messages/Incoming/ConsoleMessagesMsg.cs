using System;
using System.Collections.Generic;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed class ConsoleMessagesMsg : List<ConsoleMessage>, IMessage<ConsoleMessagesMsg>
{
    static Identifier IMessage<ConsoleMessagesMsg>.Identifier => In.NewConsole;

    static ConsoleMessagesMsg IParser<ConsoleMessagesMsg>.Parse(in PacketReader p) => p.Client switch
    {
        ClientType.Shockwave => [.. p.ParseArray<ConsoleMessage>()],
        not ClientType.Shockwave => [p.Parse<ConsoleMessage>()],
    };

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.ComposeArray(this);
        }
        else
        {
            if (Count != 1)
                throw new Exception("Only a single console message is supported on modern clients.");
            p.Compose(this[0]);
        }
    }
}
