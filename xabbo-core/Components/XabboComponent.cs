using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    public abstract class XabboComponent : IListener
    {
        public static IEnumerable<Type> GetCoreComponentTypes()
        {
            foreach (var type in typeof(XabboComponent).Assembly.GetExportedTypes())
            {
                if (type.IsSubclassOf(typeof(XabboComponent)))
                    yield return type;
            }
        }

        public ComponentManager Manager { get; internal set; }
        public bool IsAvailable { get; internal set; }
        internal bool IsFaulted { get; set; }

        protected IInterceptor Interceptor => Manager.Interceptor;
        protected MessageDispatcher Dispatcher => Interceptor.Dispatcher;
        protected Headers Headers => Dispatcher.Headers;
        protected IncomingHeaders In => Headers.Incoming;
        protected OutgoingHeaders Out => Headers.Outgoing;

        protected T GetComponent<T>() where T : XabboComponent => Manager.GetComponent<T>();

        internal void Initialize() => OnInitialize();

        protected abstract void OnInitialize();

        protected Task SendAsync(short header, params object[] values) => Interceptor.SendToServerAsync(header, values);
        protected Task SendAsync(Packet packet) => Interceptor.SendToServerAsync(packet);
        protected Task SendAsync(byte[] data) => Interceptor.SendToServerAsync(data);

        protected Task SendLocalAsync(short header, params object[] values) => Interceptor.SendToClientAsync(header, values);
        protected Task SendLocalAsync(Packet packet) => Interceptor.SendToClientAsync(packet);
        protected Task SendLocalAsync(byte[] data) => Interceptor.SendToClientAsync(data);
    }
}
