using System;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Messages
{
    public class MessageEventArgs
    {
        public MessageDispatcher Dispatcher { get; }
        public Packet Packet { get; }

        public MessageEventArgs (MessageDispatcher dispatcher, Packet packet)
        {
            Dispatcher = dispatcher;
            Packet = packet;
        }
    }
}
