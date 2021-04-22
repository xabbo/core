using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;
using Xabbo.Interceptor.Dispatcher;

namespace Xabbo.Core.Game
{
    public abstract class GameStateManager : INotifyPropertyChanged, IDisposable
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
        protected IInterceptDispatcher Dispatcher => Interceptor.Dispatcher;
        protected Incoming In => Interceptor.Messages.In;
        protected Outgoing Out => Interceptor.Messages.Out;

        public GameStateManager(IInterceptor interceptor)
        {
            Interceptor = interceptor;
            Interceptor.Dispatcher.Bind(this);
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
                Interceptor.Dispatcher.Release(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
