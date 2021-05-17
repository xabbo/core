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
        public IRoomUser? Self { get; private set; }
        public IRoomUser? Partner { get; private set; }
        public ITradeOffer? OwnOffer { get; private set; }
        public ITradeOffer? PartnerOffer { get; private set; }

        public bool HasAccepted { get; private set; }
        public bool HasPartnerAccepted { get; private set; }
        public bool IsWaitingConfirmation { get; private set; }

        public event EventHandler<TradeStartEventArgs>? Start;
        public event EventHandler<TradeStartFailEventArgs>? StartFail;
        public event EventHandler<TradeOfferEventArgs>? Update;
        public event EventHandler<TradeAcceptEventArgs>? Accept;
        public event EventHandler? WaitingConfirm;
        public event EventHandler<TradeStopEventArgs>? Stop;
        public event EventHandler<TradeCompleteEventArgs>? Complete;

        protected virtual void OnStart(bool isTrader, IRoomUser partner)
            => Start?.Invoke(this, new TradeStartEventArgs(isTrader, partner));
        protected virtual void OnStartFail(int reason, string name)
            => StartFail?.Invoke(this, new TradeStartFailEventArgs(reason, name));
        protected virtual void OnUpdate(ITradeOffer ownOffer, ITradeOffer partnerOffer)
            => Update?.Invoke(this, new TradeOfferEventArgs(ownOffer, partnerOffer));
        protected virtual void OnAccept(IRoomUser user, bool accepted)
            => Accept?.Invoke(this, new TradeAcceptEventArgs(user, accepted));
        protected virtual void OnWaitingConfirm() => WaitingConfirm?.Invoke(this, EventArgs.Empty);
        protected virtual void OnStop(IRoomUser user, int reason)
            => Stop?.Invoke(this, new TradeStopEventArgs(user, reason));
        protected virtual void OnComplete(bool wasTrader, IRoomUser self, IRoomUser partner,
            ITradeOffer ownOffer, ITradeOffer partnerOffer)
            => Complete?.Invoke(this, new TradeCompleteEventArgs(wasTrader, self, partner, ownOffer, partnerOffer));

        public TradeManager(IInterceptor interceptor, ProfileManager profileManager,
            RoomManager roomManager)
            : base(interceptor)
        {
            _profileManager = profileManager;
            _roomManager = roomManager;
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
        private void HandleTradeStart(IReadOnlyPacket packet)
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

            OnStart(IsTrader, Partner);
        }

        [Receive("TradeStartFail")]
        private void HandleTradeStartFail(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            int reason = packet.ReadInt();
            string name = packet.ReadString();

            ResetTrade();
            OnStartFail(reason, name);
        }

        [Receive("TradeUpdate")]
        private void HandleTradeUpdate(IReadOnlyPacket packet)
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

            OnUpdate(OwnOffer, PartnerOffer);
        }

        [Receive("TradeAccepted")]
        private void HandleTradeAccepted(IReadOnlyPacket packet)
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
            OnAccept(user, accepted);
        }

        [Receive("TradingWaitingConfirm")]
        private void HandleTradingWaitingConfirm(IReadOnlyPacket packet)
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

        [Receive("TradeStopped")]
        private void HandleTradeStopped(IReadOnlyPacket packet)
        {
            if (!_roomManager.IsInRoom || !IsTrading)
                return;

            int userId = packet.ReadInt();
            int reason = packet.ReadInt();

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

            IRoomUser user;
            if (userId == self.Id) user = self;
            else if (userId == partner.Id) user = partner;
            else
            {
                DebugUtil.Log($"user id {userId} does not match self {Self} or partner {Partner} ids");
                return;
            }

            ResetTrade();

            if (reason == 0)
            {
                DebugUtil.Log($"complete {userId}, partner = {partner}");
                OnComplete(wasTrader, self, partner, ownOffer, partnerOffer);
            }
            else
            {
                DebugUtil.Log($"stopped, reason = {reason} ({userId})");
                OnStop(user, reason);
            }
        }
    }
}
