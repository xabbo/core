using System;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    [Dependencies(
        typeof(RoomManager),
        typeof(EntityManager)
    )]
    public class TradeManager : XabboComponent
    {
        private RoomManager roomManager;
        private EntityManager entityManager;

        public bool IsTrading { get; private set; }
        public bool IsWaitingConfirmation { get; private set; }

        public IRoomUser Trader { get; private set; }
        public IRoomUser Tradee { get; private set; }
        public bool TraderAccepted { get; private set; }
        public bool TradeeAccepted { get; private set; }

        public ITradeOffer OwnOffer { get; private set; }
        public ITradeOffer PartnerOffer { get; private set; }

        public bool HasAccepted(IRoomUser user) => HasAccepted(user?.Id ?? throw new ArgumentNullException("user"));
        public bool HasAccepted(int userId)
        {
            if (!IsTrading) return false;
            else if (userId == Trader?.Id) return TraderAccepted;
            else if (userId == Tradee?.Id) return TradeeAccepted;
            else return false;
        }

        public ITradeOffer GetOffer(IRoomUser user) => GetOffer(user?.Id ?? throw new ArgumentNullException("user"));
        public ITradeOffer GetOffer(int userId)
        {
            if (OwnOffer?.UserId == userId) return OwnOffer;
            else if (PartnerOffer?.UserId == userId) return PartnerOffer;
            else return null;
        }

        public event EventHandler<TradeStartEventArgs> Start;
        public event EventHandler<TradeStartFailEventArgs> StartFail;
        public event EventHandler<TradeOfferEventArgs> Update;
        public event EventHandler<TradeAcceptEventArgs> Accept;
        public event EventHandler WaitingConfirm;
        public event EventHandler<TradeStopEventArgs> Stop;
        public event EventHandler<TradeOfferEventArgs> Complete;

        protected virtual void OnStart(IRoomUser trader, IRoomUser tradee)
            => Start?.Invoke(this, new TradeStartEventArgs(trader, tradee));
        protected virtual void OnStartFail(int reason, string name)
            => StartFail?.Invoke(this, new TradeStartFailEventArgs(reason, name));
        protected virtual void OnUpdate(IRoomUser trader, IRoomUser tradee, ITradeOffer ownOffer, ITradeOffer partnerOffer)
            => Update?.Invoke(this, new TradeOfferEventArgs(trader, tradee, ownOffer, partnerOffer));
        protected virtual void OnAccept(IRoomUser user, bool accepted)
            => Accept?.Invoke(this, new TradeAcceptEventArgs(user, accepted));
        protected virtual void OnWaitingConfirm() => WaitingConfirm?.Invoke(this, EventArgs.Empty);
        protected virtual void OnStop(int reason)
            => Stop?.Invoke(this, new TradeStopEventArgs(reason));
        protected virtual void OnComplete(IRoomUser trader, IRoomUser tradee, ITradeOffer ownOffer, ITradeOffer partnerOffer)
            => Complete?.Invoke(this, new TradeOfferEventArgs(trader, tradee, ownOffer, partnerOffer));

        protected override void OnInitialize()
        {
            roomManager = GetComponent<RoomManager>();
            entityManager = GetComponent<EntityManager>();
        }

        private void ResetTrade()
        {
            IsTrading =
            IsWaitingConfirmation =
            TraderAccepted =
            TradeeAccepted = false;

            Trader = null;
            Tradee = null;
            OwnOffer = null;
            PartnerOffer = null;
        }

        [Receive("TradeStart")]
        private void HandleTradeStart(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            int traderId = packet.ReadInt();
            int unknownA = packet.ReadInt(); // ?
            int tradeeId = packet.ReadInt();
            int unknownB = packet.ReadInt(); // ?

            if (!entityManager.TryGetEntity(traderId, out IRoomUser trader))
            {
                DebugUtil.Log($"failed to find user with id {traderId}");
                return;
            }

            if (!entityManager.TryGetEntity(tradeeId, out IRoomUser tradee))
            {
                DebugUtil.Log($"failed to find user with id {tradeeId}");
                return;
            }

            ResetTrade();

            IsTrading = true;
            Trader = trader;
            Tradee = tradee;

            DebugUtil.Log($"trader {Trader} {unknownA} / tradee {Tradee} {unknownB}");
            OnStart(Trader, Tradee);
        }

        [Receive("TradeStartFail")]
        private void HandleTradeStartFail(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            int reason = packet.ReadInt();
            string name = packet.ReadString();

            OnStartFail(reason, name);
        }

        [Receive("TradeUpdate")]
        private void HandleTradeUpdate(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            if (!IsTrading)
            {
                DebugUtil.Log("not trading");
                return;
            }

            TraderAccepted = false;
            TradeeAccepted = false;
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

            OnUpdate(Trader, Tradee, OwnOffer, PartnerOffer);
        }

        [Receive("TradeAccepted")]
        private void HandleTradeAccepted(Packet packet)
        {
            if (!roomManager.IsInRoom)
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

            if (userId == Trader.Id)
            {
                user = Trader;
                TraderAccepted = accepted;
            }
            else if (userId == Tradee.Id)
            {
                user = Tradee;
                TradeeAccepted = accepted;
            }
            else
            {
                DebugUtil.Log($"user id {userId} does not match trader {Trader} or tradee {Tradee} ids");
                return;
            }

            DebugUtil.Log($"user {user} {(accepted ? "" : "un")}accepted");
            OnAccept(user, accepted);
        }

        [Receive("TradingWaitingConfirm")]
        private void HandleTradingWaitingConfirm(Packet packet)
        {
            if (!roomManager.IsInRoom)
            {
                DebugUtil.Log("not in room");
                return;
            }

            if (!IsTrading)
            {
                DebugUtil.Log("not trading");
                return;
            }

            OnWaitingConfirm();
        }

        [Receive("TradeStopped")]
        private void HandleTradeStopped(Packet packet)
        {
            if (!roomManager.IsInRoom || !IsTrading)
                return;

            int userId = packet.ReadInt();
            int reason = packet.ReadInt();

            if (reason == 0)
            {
                DebugUtil.Log($"complete ({userId}), trader = {Trader}, Tradee = {Tradee}");
                OnComplete(Trader, Tradee, OwnOffer, PartnerOffer);
            }
            else
            {
                DebugUtil.Log($"stopped, reason = {reason} ({userId})");
                OnStop(reason);
            }

            ResetTrade();
        }
    }
}
