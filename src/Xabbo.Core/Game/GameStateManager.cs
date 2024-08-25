using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xabbo.Messages;
using Xabbo.Extension;

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

    protected IExtension Ext { get; }
    protected IMessageDispatcher Dispatcher => Ext.Dispatcher;

    public GameStateManager(IExtension extension)
    {
        Ext = extension;
        Ext.Connected += OnConnected;
        Ext.Disconnected += OnDisconnected;
    }

    protected virtual void OnConnected(GameConnectedArgs e)
    {
        if (this is IMessageHandler handler)
            _attachment = handler.Attach(Ext);
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
