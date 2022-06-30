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
    public abstract class GameStateManager : IInterceptHandler, INotifyPropertyChanged, IDisposable
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
        protected virtual void RaisePropertyChanged(string? propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            RaisePropertyChanged(propertyName);
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
            Interceptor.Connected += OnConnected;
            Interceptor.Disconnected += OnDisconnected;
        }

        protected virtual void OnConnected(object? sender, EventArgs e)
        {
            if (!Interceptor.Dispatcher.IsBound(this))
            {
                Interceptor.Bind(this);
            }
        }

        protected virtual void OnDisconnected(object? sender, EventArgs e)
        {
            Interceptor.Release(this);
        }

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
