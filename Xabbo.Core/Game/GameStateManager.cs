using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Game
{
    public abstract class GameStateManager : INotifyPropertyChanged, IListener, IDisposable
    {
        public static IEnumerable<Type> GetManagerTypes()
        {
            foreach (var type in typeof(GameStateManager).Assembly.GetExportedTypes())
            {
                if (type.IsSubclassOf(typeof(GameStateManager)))
                    yield return type;
            }
        }

        #region - INotifyPropertyChanged -
        public event PropertyChangedEventHandler? PropertyChanged;
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

        private bool _disposed;

        protected IInterceptor Interceptor { get; }
        protected MessageDispatcher Dispatcher => Interceptor.Dispatcher;
        protected Headers Headers => Dispatcher.Headers;
        protected Incoming In => Headers.Incoming;
        protected Outgoing Out => Headers.Outgoing;

        public GameStateManager(IInterceptor interceptor)
        {
            Interceptor = interceptor;
            Interceptor.Dispatcher.Attach(this);
        }

        protected Task SendAsync(Header header, params object[] values) => Interceptor.SendToServerAsync(header, values);
        protected Task SendAsync(IReadOnlyPacket packet) => Interceptor.SendToServerAsync(packet);

        protected Task SendLocalAsync(Header header, params object[] values) => Interceptor.SendToClientAsync(header, values);
        protected Task SendLocalAsync(IReadOnlyPacket packet) => Interceptor.SendToClientAsync(packet);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                Interceptor.Dispatcher.Detach(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
