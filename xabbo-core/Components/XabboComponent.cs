using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    public abstract class XabboComponent : INotifyPropertyChanged, IListener
    {
        public static IEnumerable<Type> GetCoreComponentTypes()
        {
            foreach (var type in typeof(XabboComponent).Assembly.GetExportedTypes())
            {
                if (type.IsSubclassOf(typeof(XabboComponent)))
                    yield return type;
            }
        }

        #region - INotifyPropertyChanged -
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool _set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        public ComponentManager Manager { get; internal set; }

        private bool isAvailable;
        public bool IsAvailable
        {
            get => isAvailable;
            internal set => _set(ref isAvailable, value);
        }
        internal bool IsFaulted { get; set; }

        protected IInterceptor Interceptor => Manager.Interceptor;
        protected MessageDispatcher Dispatcher => Interceptor.Dispatcher;
        protected Headers Headers => Dispatcher.Headers;
        protected IncomingHeaders In => Headers.Incoming;
        protected OutgoingHeaders Out => Headers.Outgoing;

        protected T GetComponent<T>() where T : XabboComponent => Manager.GetComponent<T>();

        internal void Initialize() => OnInitialize();

        protected internal virtual bool CheckAvailability() => true;
        protected abstract void OnInitialize();

        protected Task SendAsync(short header, params object[] values) => Interceptor.SendToServerAsync(header, values);
        protected Task SendAsync(Packet packet) => Interceptor.SendToServerAsync(packet);

        protected Task SendLocalAsync(short header, params object[] values) => Interceptor.SendToClientAsync(header, values);
        protected Task SendLocalAsync(Packet packet) => Interceptor.SendToClientAsync(packet);
    }
}
