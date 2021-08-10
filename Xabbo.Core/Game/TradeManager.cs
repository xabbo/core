using System;

using Xabbo.Messages;
using Xabbo.Interceptor;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game
{
    public class TradeManager : GameStateManager
    {
        private readonly ProfileManager _profileManager;
        private readonly RoomManager _roomManager;

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

        protected virtual void OnOpened(bool isTrader, IRoomUser partner)
            => Opened?.Invoke(this, new TradeStartEventArgs(isTrader, partner));
        protected virtual void OnTradeOpenFailed(int reason, string name)
            => OpenFailed?.Invoke(this, new TradeStartFailEventArgs(reason, name));
        protected virtual void OnUpdated(ITradeOffer ownOffer, ITradeOffer partnerOffer)
            => Updated?.Invoke(this, new TradeOfferEventArgs(ownOffer, partnerOffer));
        protected virtual void OnAccepted(IRoomUser user, bool accepted)
            => Accepted?.Invoke(this, new TradeAcceptEventArgs(user, accepted));
        protected virtual void OnWaitingConfirm() => WaitingConfirm?.Invoke(this, EventArgs.Empty);
        protected virtual void OnClosed(IRoomUser user, int reason)
            => Closed?.Invoke(this, new TradeStopEventArgs(user, reason));
        protected virtual void OnCompleted(bool wasTrader, IRoomUser self, IRoomUser partner,
            ITradeOffer ownOffer, ITradeOffer partnerOffer)
            => Completed?.Invoke(this, new TradeCompleteEventArgs(wasTrader, self, partner, ownOffer, partnerOffer));

        public TradeManager(IInterceptor interceptor, ProfileManager profileManager,
            RoomManager roomManager)
            : base(interceptor)
        {
            _profileManager = profileManager;
            _roomManager = roomManager;
        }

        protected override void OnDisconnected(object? sender, EventArgs e)
        {
            base.OnDisconnected(sender, e);

            ResetTrade();
        }

        private void ResetTrade()
        {
            IsTrading =
            IsTrader =
            HasAccepted = 
            HasPartnerAccepted =
            IsWaitingConfirmation = false;

            Self =
            Partner = null;

            OwnOffer =
            PartnerOffer = null;
        }

        [Receive(nameof(Incoming.TradeOpen))]
        private void HandleTradeOpen(IReadOnlyPacket packet)
        {
            if (_profileManager.UserData == null)
            {
                DebugUtil.Log("user data not loaded");
                return;
            }

            if (!_roomManager.IsInRoom || _roomManager.Room is null)
            {
                DebugUtil.Log("not in room");
                return;
            }

            if (IsTrading)
            {
                DebugUtil.Log("already trading!");
                return;
            }

            int traderId = packet.ReadInt();
            int unknownA = packet.ReadInt(); // ?
            int tradeeId = packet.ReadInt();
            int unknownB = packet.ReadInt(); // ?

            if (!_roomManager.Room.TryGetEntityById(traderId, out IRoomUser? trader))
            {
                DebugUtil.Log($"failed to find user with id {traderId}");
                return;
            }

            if (!_roomManager.Room.TryGetEntityById(tradeeId, out IRoomUser? tradee))
            {
                DebugUtil.Log($"failed to find user with id {tradeeId}");
                return;
            }

            ResetTrade();

            IsTrader = _profileManager.UserData.Id == traderId;
            Self = IsTrader ? trader : tradee;
            Partner = IsTrader ? tradee : trader;

            IsTrading = true;

            DebugUtil.Log($"trade opened with {Partner}");
            OnOpened(IsTrader, Partner);
        }

        [Receive(nameof(Incoming.TradeOpenFail))]
        private void HandleTradeOpenFail(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            int reason = packet.ReadInt();
            string name = packet.ReadString();

            OnTradeOpenFailed(reason, name);
        }

        [Receive(nameof(Incoming.TradeItems))]
        private void HandleTradeItems(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            if (!IsTrading)
            {
                DebugUtil.Log("not trading");
                return;
            }

            HasAccepted = 
            HasPartnerAccepted = false;
            OwnOffer = TradeOffer.Parse(packet);
            PartnerOffer = TradeOffer.Parse(packet);

            DebugUtil.Log(
                $"user {OwnOffer.UserId}: " +
                $"{OwnOffer.FurniCount} furni, " +
                $"{OwnOffer.CreditCount} credits / " +
                $"user {PartnerOffer.UserId}: " +
                $"{PartnerOffer.FurniCount} furni, " +
                $"{PartnerOffer.CreditCount} credits"
            );

            OnUpdated(OwnOffer, PartnerOffer);
        }

        [Receive(nameof(Incoming.TradeAccept))]
        private void HandleTradeAccept(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            if (!IsTrading)
            {
                DebugUtil.Log("not trading");
                return;
            }

            IRoomUser user;
            int userId = packet.ReadInt();
            bool accepted = packet.ReadInt() == 1;

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
                DebugUtil.Log($"user id {userId} does not match self {Self} or partner {Partner} ids");
                return;
            }

            DebugUtil.Log($"user {user} {(accepted ? "" : "un")}accepted");
            OnAccepted(user, accepted);
        }

        [Receive(nameof(Incoming.TradeConfirmation))]
        private void HandleTradeConfirmation(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            if (!IsTrading)
            {
                DebugUtil.Log("not trading");
                return;
            }

            IsWaitingConfirmation = true;
            OnWaitingConfirm();
        }

        [Receive(nameof(Incoming.TradeCompleted))]
        private void HandleTradeCompleted(IReadOnlyPacket packet)
        {
            if (!IsTrading) return;

            if (IsCompleted)
            {
                DebugUtil.Log($"trade already complete!");
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

            DebugUtil.Log($"complete, partner = {partner}");

            IsCompleted = true;
            OnCompleted(wasTrader, self, partner, ownOffer, partnerOffer);
        }

        [Receive(nameof(Incoming.TradeClose))]
        private void HandleTradeClose(IReadOnlyPacket packet)
        {
            if (!IsTrading) return;

            int userId = packet.ReadInt();
            int reason = packet.ReadInt();

            IRoomUser? user = null;
            if (userId == Self?.Id) user = Self;
            else if (userId == Partner?.Id) user = Partner;
            
            if (user is null)
            {
                DebugUtil.Log($"user id mismatch: {userId}.");
                return;
            }

            DebugUtil.Log($"trade closed by {user}, reason = {reason}");
            OnClosed(user, reason);
            ResetTrade();
        }

    }
}
