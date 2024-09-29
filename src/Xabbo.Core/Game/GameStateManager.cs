using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xabbo.Messages;
using Xabbo.Messages.Flash;
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

    private bool _connectedOnInit;
    private bool _hasInitialized;
    private IDisposable? _pingIntercept;

    protected IInterceptor Interceptor { get; }
    protected Session Session => Interceptor.Session;
    protected IMessageDispatcher Dispatcher => Interceptor.Dispatcher;

    public GameStateManager(IInterceptor interceptor)
    {
        Interceptor = interceptor;
        Interceptor.Initialized += OnInitialized;
        Interceptor.Connected += OnConnected;
        Interceptor.Disconnected += OnDisconnected;
    }

    private void OnInitialized(InitializedEventArgs e)
    {
        _hasInitialized = false;
        _connectedOnInit = e.IsGameConnected ?? false;
    }

    protected virtual void OnConnected(ConnectedEventArgs e)
    {
        if (this is IMessageHandler handler)
            _attachment = handler.Attach(Interceptor);

        if (_connectedOnInit && !_hasInitialized)
        {
            OnInitialize(true);
            _hasInitialized = true;
        }
        else
        {
            if (e.Session.Client.Type is ClientType.Shockwave)
            {
                _pingIntercept = Interceptor.Intercept(In.Ping, HandlePing);
            }
            else
            {
                _pingIntercept = Interceptor.Intercept(In.LatencyPingResponse, HandlePing);
            }
        }
    }

    private void HandlePing(Intercept e)
    {
        if (e.Sequence >= 10)
        {
            if (!_hasInitialized)
            {
                OnInitialize(false);
                _hasInitialized = true;
            }
            _pingIntercept?.Dispose();
        }
    }

    protected virtual void OnDisconnected()
    {
        _hasInitialized = false;
        _connectedOnInit = false;
    }

    /// <summary>
    /// Called when it is probably safe to initialize the state manager.
    /// </summary>
    protected virtual void OnInitialize(bool initializingAfterConnect) { }

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
