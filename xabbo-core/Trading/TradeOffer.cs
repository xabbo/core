using System;
using System.Collections.Generic;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class TradeOffer
    {
        public static TradeOffer Parse(Packet packet) => new TradeOffer(packet);

        public int UserId { get; set; }
        public List<TradeItem> Items { get; set; }
        public int FurniCount { get; set; }
        public int CreditCount { get; set; }

        public TradeOffer()
        {
            Items = new List<TradeItem>();
        }

        protected TradeOffer(Packet packet)
            : this()
        {
            UserId = packet.ReadInteger();
            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                Items.Add(TradeItem.Parse(packet));
            FurniCount = packet.ReadInteger();
            CreditCount = packet.ReadInteger();
        }
    }
}
