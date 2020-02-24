using System;
using System.Collections.Generic;

using Xabbo.Core.Messages;

namespace Xabbo.Core.Components
{
    public abstract class XabboComponent : IListener
    {
        public static IEnumerable<Type> GetCoreComponentTypes()
        {
            foreach (var type in typeof(XabboComponent).Assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(XabboComponent)))
                    yield return type;
            }
        }

        public ComponentManager Manager { get; internal set; }
        public bool IsAvailable { get; internal set; }

        public IInterceptor Interceptor => Manager.Interceptor;
        public MessageDispatcher Dispatcher => Interceptor.Dispatcher;
        public Headers Headers => Dispatcher.Headers;
        public IncomingHeaders In => Headers.Incoming;
        public OutgoingHeaders Out => Headers.Outgoing;

        protected T GetComponent<T>() where T : XabboComponent => Manager.GetComponent<T>();

        internal void Initialize() => OnInitialize();

        protected abstract void OnInitialize();
    }
}
