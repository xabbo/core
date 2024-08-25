using System;

using Xabbo.Messages;
using Xabbo.Extension;
using Xabbo.Interceptor;

using Xabbo.Core.Events;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Game;

public sealed class TradeManager(IExtension extension, ProfileManager profileManager, RoomManager roomManager)
    : GameStateManager(extension)
{
    private readonly ProfileManager _profileManager = profileManager;
    private readonly RoomManager _roomManager = roomManager;

    public bool IsTrading { get; private set; }
    public bool IsTrader { get; private set; }
    public bool IsCompleted { get; private set; }
    public IRoomUser? Self { get; private set; }
    public IRoomUser? Partner { get; private set; }
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

    private void OnOpened(bool isTrader, IRoomUser partner)
        => Opened?.Invoke(this, new TradeStartEventArgs(isTrader, partner));
    private void OnTradeOpenFailed(int reason, string name)
        => OpenFailed?.Invoke(this, new TradeStartFailEventArgs(reason, name));
    private void OnUpdated(ITradeOffer ownOffer, ITradeOffer partnerOffer)
        => Updated?.Invoke(this, new TradeOfferEventArgs(ownOffer, partnerOffer));
    private void OnAccepted(IRoomUser user, bool accepted)
        => Accepted?.Invoke(this, new TradeAcceptEventArgs(user, accepted));
    private void OnWaitingConfirm() => WaitingConfirm?.Invoke(this, EventArgs.Empty);
    private void OnClosed(IRoomUser user, int reason)
        => Closed?.Invoke(this, new TradeStopEventArgs(user, reason));
    private void OnCompleted(bool wasTrader, IRoomUser self, IRoomUser partner,
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

    // [Receive(nameof(In.TradeOpen))]
    private void HandleTradeOpen(Intercept e)
    {
        if (_profileManager.UserData == null)
        {
            Debug.Log("user data not loaded");
            return;
        }

        if (!_roomManager.IsInRoom || _roomManager.Room is null)
        {
            Debug.Log("not in room");
            return;
        }

        if (IsTrading)
        {
            Debug.Log("already trading!");
            return;
        }

        int traderId = e.Packet.Read<int>();
        int traderCanTrade = e.Packet.Read<int>();
        int tradeeId = e.Packet.Read<int>();
        int tradeeCanTrade = e.Packet.Read<int>();

        if (!_roomManager.Room.TryGetEntityById(traderId, out IRoomUser? trader))
        {
            Debug.Log($"failed to find user with id {traderId}");
            return;
        }

        if (!_roomManager.Room.TryGetEntityById(tradeeId, out IRoomUser? tradee))
        {
            Debug.Log($"failed to find user with id {tradeeId}");
            return;
        }

        ResetTrade();

        IsTrader = _profileManager.UserData.Id == traderId;
        Self = IsTrader ? trader : tradee;
        Partner = IsTrader ? tradee : trader;

        IsTrading = true;

        Debug.Log($"trade opened with {Partner}");
        OnOpened(IsTrader, Partner);
    }

    // [Receive(nameof(In.TradeOpenFail))]
    private void HandleTradeOpenFail(Intercept e)
    {
        if (!_roomManager.IsInRoom)
        {
            Debug.Log("not in room");
            return;
        }

        int reason = e.Packet.Read<int>();
        string name = e.Packet.Read<string>();

        OnTradeOpenFailed(reason, name);
    }

    // [Receive(nameof(In.TradeItems))]
    private void HandleTradeItems(Intercept e)
    {
        if (!_roomManager.IsInRoom)
        {
            Debug.Log("not in room");
            return;
        }

        if (!IsTrading)
        {
            Debug.Log("not trading");
            return;
        }

        HasAccepted =
        HasPartnerAccepted = false;
        OwnOffer = e.Packet.Parse<TradeOffer>();
        PartnerOffer = e.Packet.Parse<TradeOffer>();

        Debug.Log(
            $"user {OwnOffer.UserId}: " +
            $"{OwnOffer.FurniCount} furni, " +
            $"{OwnOffer.CreditCount} credits / " +
            $"user {PartnerOffer.UserId}: " +
            $"{PartnerOffer.FurniCount} furni, " +
            $"{PartnerOffer.CreditCount} credits"
        );

        OnUpdated(OwnOffer, PartnerOffer);
    }

    // [Receive(nameof(In.TradeAccept))]
    private void HandleTradeAccept(Intercept e)
    {
        if (!_roomManager.IsInRoom)
        {
            Debug.Log("not in room");
            return;
        }

        if (!IsTrading)
        {
            Debug.Log("not trading");
            return;
        }

        IRoomUser user;
        int userId = e.Packet.Read<int>();
        bool accepted = e.Packet.Read<int>() == 1;

        if (userId == Self?.Id)
        {
            user = Self;
            HasAccepted = accepted;
        }
        else if (userId == Partner?.Id)
        {
            user = Partner;
            HasPartnerAccepted = accepted;
        }
        else
        {
            Debug.Log($"user id {userId} does not match self {Self} or partner {Partner} ids");
            return;
        }

        Debug.Log($"user {user} {(accepted ? "" : "un")}accepted");
        OnAccepted(user, accepted);
    }

    // [Receive(nameof(In.TradeConfirmation))]
    private void HandleTradeConfirmation(Intercept e)
    {
        if (!_roomManager.IsInRoom)
        {
            Debug.Log("not in room");
            return;
        }

        if (!IsTrading)
        {
            Debug.Log("not trading");
            return;
        }

        IsWaitingConfirmation = true;
        OnWaitingConfirm();
    }

    // [Receive(nameof(In.TradeCompleted))]
    private void HandleTradeCompleted(Intercept e)
    {
        if (!IsTrading) return;

        if (IsCompleted)
        {
            Debug.Log($"trade already complete!");
            return;
        }

        bool wasTrader = IsTrader;
        IRoomUser? self = Self;
        IRoomUser? partner = Partner;
        ITradeOffer? ownOffer = OwnOffer;
        ITradeOffer? partnerOffer = PartnerOffer;

        if (self == null || partner == null ||
            ownOffer == null || partnerOffer == null)
        {
            return;
        }

        Debug.Log($"complete, partner = {partner}");

        IsCompleted = true;
        OnCompleted(wasTrader, self, partner, ownOffer, partnerOffer);
    }

    // [Receive(nameof(In.TradeClose))]
    private void HandleTradeClose(Intercept e)
    {
        if (!IsTrading) return;

        int userId = e.Packet.Read<int>();
        int reason = e.Packet.Read<int>();

        IRoomUser? user = null;
        if (userId == Self?.Id) user = Self;
        else if (userId == Partner?.Id) user = Partner;

        if (user is null)
        {
            Debug.Log($"user id mismatch: {userId}.");
            return;
        }

        Debug.Log($"trade closed by {user}, reason = {reason}");
        OnClosed(user, reason);
        ResetTrade();
    }
}
