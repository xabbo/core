using System;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Extension;
using Xabbo.Messages.Flash;
using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

[Intercept(~ClientType.Shockwave)]
public sealed partial class TradeManager(
    IExtension extension, ProfileManager profileManager, RoomManager roomManager, ILoggerFactory? loggerFactory = null
)
    : GameStateManager(extension)
{
    private readonly ILogger Log = (ILogger?)loggerFactory?.CreateLogger<ProfileManager>() ?? NullLogger.Instance;

    private readonly ProfileManager _profileManager = profileManager;
    private readonly RoomManager _roomManager = roomManager;

    public bool IsTrading { get; private set; }
    public bool IsTrader { get; private set; }
    public bool IsCompleted { get; private set; }
    public IUser? Self { get; private set; }
    public IUser? Partner { get; private set; }
    public ITradeOffer? OwnOffer { get; private set; }
    public ITradeOffer? PartnerOffer { get; private set; }

    public bool HasAccepted { get; private set; }
    public bool HasPartnerAccepted { get; private set; }
    public bool IsWaitingConfirmation { get; private set; }

    public event EventHandler<TradeStartEventArgs>? Opened;
    public event EventHandler<TradeStartFailEventArgs>? OpenFailed;
    public event EventHandler<TradeOfferEventArgs>? Updated;
    public event EventHandler<TradeAcceptEventArgs>? Accepted;
    public event EventHandler? WaitingConfirm;
    public event EventHandler<TradeStopEventArgs>? Closed;
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
