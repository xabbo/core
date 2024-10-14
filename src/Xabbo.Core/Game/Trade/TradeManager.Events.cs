using System;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

partial class TradeManager
{
    /// <summary>
    /// Occurs when a trade is opened.
    /// </summary>
    public event Action<TradeOpenedEventArgs>? Opened;

    /// <summary>
    /// Occurs when a trade fails to open.
    /// </summary>
    /// <remarks>
    /// Available on modern clients.
    /// </remarks>
    public event Action<TradeOpenFailedEventArgs>? OpenFailed;

    /// <summary>
    /// Occurs when a trade is updated.
    /// </summary>
    public event Action<TradeUpdatedEventArgs>? Updated;

    /// <summary>
    /// Occurs when a trade is accepted by one of the users.
    /// </summary>
    public event Action<TradeAcceptedEventArgs>? Accepted;

    /// <summary>
    /// Occurs when a trade is awaiting confirmation.
    /// </summary>
    /// <remarks>
    /// Available on modern clients.
    /// </remarks>
    public event Action? AwaitingConfirmation;

    /// <summary>
    /// Occurs when a trade is closed.
    /// </summary>
    public event Action<TradeClosedEventArgs>? Closed;

    /// <summary>
    /// Occurs when a trade completes successfully.
    /// </summary>
    public event Action<TradeCompletedEventArgs>? Completed;
}