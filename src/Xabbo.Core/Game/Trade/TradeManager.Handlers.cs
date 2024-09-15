using Xabbo.Messages.Flash;

namespace Xabbo.Core.Game;

partial class TradeManager
{
    [InterceptIn(nameof(In.TradingOpen))]
    private void HandleTradingOpen(Intercept e)
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

        if (!_roomManager.Room.TryGetAvatarById(traderId, out IUser? trader))
        {
            Debug.Log($"failed to find user with id {traderId}");
            return;
        }

        if (!_roomManager.Room.TryGetAvatarById(tradeeId, out IUser? tradee))
        {
            Debug.Log($"failed to find user with id {tradeeId}");
            return;
        }

        ResetTrade();

        IsTrader = ((IUserData)_profileManager.UserData).Id == traderId;
        Self = IsTrader ? trader : tradee;
        Partner = IsTrader ? tradee : trader;

        IsTrading = true;

        Debug.Log($"trade opened with {Partner}");
        OnOpened(IsTrader, Partner);
    }

    [InterceptIn(nameof(In.TradeOpenFailed))]
    private void HandleTradeOpenFailed(Intercept e)
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

    [InterceptIn(nameof(In.TradingItemList))]
    private void HandleTradingItemList(Intercept e)
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
        OwnOffer = e.Packet.Read<TradeOffer>();
        PartnerOffer = e.Packet.Read<TradeOffer>();

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

    [InterceptIn(nameof(In.TradingAccept))]
    private void HandleTradingAccept(Intercept e)
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

        IUser user;
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

    [InterceptIn(nameof(In.TradingConfirmation))]
    private void HandleTradingConfirmation(Intercept e)
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

    [InterceptIn(nameof(In.TradingCompleted))]
    void HandleTradingCompleted(Intercept e)
    {
        if (!IsTrading) return;

        if (IsCompleted)
        {
            Debug.Log($"trade already complete!");
            return;
        }

        bool wasTrader = IsTrader;
        IUser? self = Self;
        IUser? partner = Partner;
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

    [InterceptIn(nameof(In.TradingClose))]
    void HandleTradingClose(Intercept e)
    {
        if (!IsTrading) return;

        int userId = e.Packet.Read<int>();
        int reason = e.Packet.Read<int>();

        IUser? user = null;
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