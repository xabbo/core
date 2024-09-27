using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Extension;
using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

/// <summary>
/// Manages the user's trading state.
/// </summary>
[Intercept(~ClientType.Shockwave)]
public sealed partial class TradeManager(
    IExtension extension, ProfileManager profileManager, RoomManager roomManager, ILoggerFactory? loggerFactory = null
)
    : GameStateManager(extension)
{
    private readonly ILogger Log = (ILogger?)loggerFactory?.CreateLogger<ProfileManager>() ?? NullLogger.Instance;

    private readonly ProfileManager _profileManager = profileManager;
    private readonly RoomManager _roomManager = roomManager;

    /// <summary>
    /// Gets whether the user is currently trading.
    /// </summary>
    public bool IsTrading { get; private set; }

    /// <summary>
    /// Gets whether the user initiated the current trade.
    /// </summary>
    public bool IsTrader { get; private set; }

    /// <summary>
    /// Gets whether the trade has completed.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Gets the user's own instance.
    /// </summary>
    public IUser? Self { get; private set; }

    /// <summary>
    /// Gets the trading parter's user instance.
    /// </summary>
    public IUser? Partner { get; private set; }

    /// <summary>
    /// Gets the user's own trade offer.
    /// </summary>
    public ITradeOffer? OwnOffer { get; private set; }

    /// <summary>
    /// Gets the trading partner's offer.
    /// </summary>
    public ITradeOffer? PartnerOffer { get; private set; }

    /// <summary>
    /// Gets whether the user has accepted the current trade.
    /// </summary>
    public bool HasAccepted { get; private set; }

    /// <summary>
    /// Gets whether the trading partner has accepted the current trade.
    /// </summary>
    public bool HasPartnerAccepted { get; private set; }

    /// <summary>
    /// Gets whether the trade is awaiting confirmation of both users.
    /// </summary>
    public bool IsWaitingConfirmation { get; private set; }

    /// <summary>
    /// Invoked when a trade is opened.
    /// </summary>
    public event EventHandler<TradeStartEventArgs>? Opened;

    /// <summary>
    /// Invoked when a trade fails to open.
    /// </summary>
    public event EventHandler<TradeStartFailEventArgs>? OpenFailed;

    /// <summary>
    /// Invoked when a trade is updated.
    /// </summary>
    public event EventHandler<TradeOfferEventArgs>? Updated;

    /// <summary>
    /// Invoked when a trade is accepted.
    /// </summary>
    public event EventHandler<TradeAcceptEventArgs>? Accepted;

    /// <summary>
    /// Invoked when a trade is awaiting confirmation.
    /// </summary>
    public event EventHandler? WaitingConfirm;

    /// <summary>
    /// Invoked when a trade is closed.
    /// </summary>
    public event EventHandler<TradeStopEventArgs>? Closed;

    /// <summary>
    /// Invoked when a trade is completed.
    /// </summary>
    public event EventHandler<TradeCompleteEventArgs>? Completed;

    private void OnOpened(bool isTrader, IUser partner)
        => Opened?.Invoke(this, new TradeStartEventArgs(isTrader, partner));
    private void OnTradeOpenFailed(int reason, string name)
        => OpenFailed?.Invoke(this, new TradeStartFailEventArgs(reason, name));
    private void OnUpdated(ITradeOffer ownOffer, ITradeOffer partnerOffer)
        => Updated?.Invoke(this, new TradeOfferEventArgs(ownOffer, partnerOffer));
    private void OnAccepted(IUser user, bool accepted)
        => Accepted?.Invoke(this, new TradeAcceptEventArgs(user, accepted));
    private void OnWaitingConfirm() => WaitingConfirm?.Invoke(this, EventArgs.Empty);
    private void OnClosed(IUser user, int reason)
        => Closed?.Invoke(this, new TradeStopEventArgs(user, reason));
    private void OnCompleted(bool wasTrader, IUser self, IUser partner,
        ITradeOffer ownOffer, ITradeOffer partnerOffer)
        => Completed?.Invoke(this, new TradeCompleteEventArgs(wasTrader, self, partner, ownOffer, partnerOffer));

    protected override void OnDisconnected() => ResetTrade();

    private void ResetTrade()
    {
        IsTrading =
        IsTrader =
        HasAccepted =
        HasPartnerAccepted =
        IsWaitingConfirmation =
        IsCompleted = false;

        Self =
        Partner = null;

        OwnOffer =
        PartnerOffer = null;
    }
}
