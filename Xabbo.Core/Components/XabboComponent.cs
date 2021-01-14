using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    public abstract class XabboComponent : INotifyPropertyChanged, IListener, IDisposable
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

        private bool isInitialized;

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
        protected Incoming In => Headers.Incoming;
        protected Outgoing Out => Headers.Outgoing;

        protected T? GetComponent<T>() where T : XabboComponent => Manager.GetComponent<T>();

        internal void Initialize()
        {
            if (isInitialized)
                throw new InvalidOperationException($"The component {GetType().FullName} has already been initialized");
            isInitialized = true;

            OnInitialize();
        }
        internal bool CheckAvailability() => OnCheckAvailability();

        protected virtual bool OnCheckAvailability() => true;
        protected abstract void OnInitialize();

        protected Task SendAsync(Header header, params object[] values) => Interceptor.SendToServerAsync(header, values);
        protected Task SendAsync(IReadOnlyPacket packet) => Interceptor.SendToServerAsync(packet);

        protected Task SendLocalAsync(Header header, params object[] values) => Interceptor.SendToClientAsync(header, values);
        protected Task SendLocalAsync(IReadOnlyPacket packet) => Interceptor.SendToClientAsync(packet);

        protected virtual void Dispose(bool disposing) { }
        public void Dispose() => Dispose(true);
    }
}
