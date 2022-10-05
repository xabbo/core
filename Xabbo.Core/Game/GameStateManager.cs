using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xabbo.Messages;
using Xabbo.Messages.Dispatcher;
using Xabbo.Extension;

namespace Xabbo.Core.Game;

public abstract class GameStateManager : IMessageHandler, INotifyPropertyChanged, IDisposable
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

    protected IExtension Extension { get; }
    protected IMessageDispatcher Dispatcher => Extension.Dispatcher;
    protected Incoming In => Extension.Messages.In;
    protected Outgoing Out => Extension.Messages.Out;

    public GameStateManager(IExtension extension)
    {
        Extension = extension;
        Extension.Connected += OnConnected;
        Extension.Disconnected += OnDisconnected;
    }

    protected virtual void OnConnected(object? sender, GameConnectedEventArgs e)
    {
        if (!Extension.Dispatcher.IsBound(this))
        {
            Extension.Bind(this);
        }
    }

    protected virtual void OnDisconnected(object? sender, EventArgs e)
    {
        Extension.Release(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        _disposed = true;

        if (disposing)
        {
            Extension.Dispatcher.Release(this);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
