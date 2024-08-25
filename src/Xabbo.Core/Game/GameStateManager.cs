using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xabbo.Messages;
using Xabbo.Interceptor;

namespace Xabbo.Core.Game;

public abstract class GameStateManager : INotifyPropertyChanged, IDisposable
{
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
    private IDisposable? _attachment;

    protected IInterceptor Interceptor { get; }
    protected IMessageDispatcher Dispatcher => Interceptor.Dispatcher;

    public GameStateManager(IInterceptor interceptor)
    {
        Interceptor = interceptor;
        Interceptor.Connected += OnConnected;
        Interceptor.Disconnected += OnDisconnected;
    }

    protected virtual void OnConnected(GameConnectedArgs e)
    {
        if (this is IMessageHandler handler)
            _attachment = handler.Attach(Interceptor);
    }

    protected virtual void OnDisconnected() { }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        _disposed = true;

        if (disposing)
        {
            _attachment?.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
