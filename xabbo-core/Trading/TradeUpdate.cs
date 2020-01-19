using System;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class TradeUpdate
    {
        public static TradeUpdate Parse(Packet packet) => new TradeUpdate(packet);

        public TradeOffer Self { get; set; }
        public TradeOffer Partner { get; set; }

        public TradeUpdate()
        {
            Self = new TradeOffer();
            Partner = new TradeOffer();
        }

        protected TradeUpdate(Packet packet)
        {
            Self = TradeOffer.Parse(packet);
            Partner = TradeOffer.Parse(packet);
        }
    }
}
