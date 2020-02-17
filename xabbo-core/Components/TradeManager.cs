using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabbo.Core.Events;
using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    [Dependencies(
        typeof(EntityManager)
    )]
    public class TradeManager : XabboComponent
    {
        private EntityManager entityManager;

        public bool IsTrading { get; private set; }

        public RoomUser Self { get; private set; }
        public RoomUser Partner { get; private set; }
        public bool SelfAccepted { get; private set; }
        public bool PartnerAccepted { get; private set; }

        public TradeUpdate CurrentUpdate { get; private set; }

        public event EventHandler<TradeStartEventArgs> Start;
        public event EventHandler<TradeUpdateEventArgs> Update;
        public event EventHandler<TradeAcceptEventArgs> Accept;
        public event EventHandler<TradeStopEventArgs> Stop;
        public event EventHandler<TradeCompleteEventArgs> Complete;

        protected virtual void OnStart(RoomUser self, RoomUser partner)
            => Start?.Invoke(this, new TradeStartEventArgs(self, partner));
        protected virtual void OnUpdate(TradeUpdate update)
            => Update?.Invoke(this, new TradeUpdateEventArgs(update));
        protected virtual void OnAccept(RoomUser user, bool accepted)
            => Accept?.Invoke(this, new TradeAcceptEventArgs(user, accepted));
        protected virtual void OnStop(int reason)
            => Stop?.Invoke(this, new TradeStopEventArgs(reason));
        protected virtual void OnComplete(RoomUser self, RoomUser partner, TradeUpdate update)
            => Complete?.Invoke(this, new TradeCompleteEventArgs(self, partner, update));

        protected override void OnInitialize()
        {
            entityManager = GetComponent<EntityManager>();
        }

        private void ResetTrade()
        {
            IsTrading = false;
            Self =
            Partner = null;
            SelfAccepted =
            PartnerAccepted = false;
        }

        [Receive("TradeStart")]
        private void HandleTradeStart(Packet packet)
        {
            int selfId = packet.ReadInteger();
            int unknownA = packet.ReadInteger(); // ?
            int partnerId = packet.ReadInteger();
            int unknownB = packet.ReadInteger(); // ?

            DebugUtil.Log($"{selfId}/{unknownA} {partnerId}/{unknownB}");

            if (!entityManager.TryGetEntityById(selfId, out RoomUser self))
            {
                DebugUtil.Log($"failed to find user with id {selfId}");
                return;
            }

            if (!entityManager.TryGetEntityById(partnerId, out RoomUser partner))
            {
                DebugUtil.Log($"failed to find user with id {partnerId}");
                return;
            }

            IsTrading = true;
            Self = self;
            Partner = partner;

            DebugUtil.Log($"start self {Self} / partner {Partner}");
            OnStart(Self, Partner);
        }

        [Receive("TradeUpdate")]
        private void HandleTradeUpdate(Packet packet)
        {
            if (!IsTrading)
            {
                DebugUtil.Log("IsTrading = false");
                return;
            }

            SelfAccepted = false;
            PartnerAccepted = false;
            CurrentUpdate = TradeUpdate.Parse(packet);

            DebugUtil.Log($"update self/" +
                $"{CurrentUpdate.Self.FurniCount} furni/" +
                $"{CurrentUpdate.Self.CreditCount} credits, partner/" +
                $"{CurrentUpdate.Partner.FurniCount} furni/" +
                $"{CurrentUpdate.Partner.CreditCount} credits"
            );
            OnUpdate(CurrentUpdate);
        }

        [Receive("TradeAccepted")]
        private void HandleTradeAccepted(Packet packet)
        {
            if (!IsTrading)
            {
                DebugUtil.Log("IsTrading = false");
                return;
            }

            RoomUser user;
            int userId = packet.ReadInteger();
            bool accepted = packet.ReadInteger() == 1;

            if (userId == Self.Id)
            {
                user = Self;
                SelfAccepted = accepted;
            }
            else if (userId == Partner.Id)
            {
                user = Partner;
                PartnerAccepted = accepted;
            }
            else
            {
                DebugUtil.Log($"user id {userId} does not match self ({Self.Id}) or partner ({Partner.Id}) ids");
                return;
            }

            DebugUtil.Log($"user {user} {(accepted ? "" : "un")}accepted");
            OnAccept(user, accepted);
        }

        [Receive("TradeStopped")]
        private void HandleTradeStopped(Packet packet)
        {
            if (!IsTrading)
            {
                DebugUtil.Log("IsTrading = false");
                return;
            }

            int userId = packet.ReadInteger();
            int reason = packet.ReadInteger();

            if (reason == 0)
            {
                DebugUtil.Log($"complete ({userId})");
                OnComplete(Self, Partner, CurrentUpdate);
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
