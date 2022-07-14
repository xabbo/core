using System;
using System.Collections.Generic;
using Xabbo.Messages;

namespace Xabbo.Core;

public class TradeOffer : ITradeOffer
{
    public static TradeOffer Parse(IReadOnlyPacket packet) => new TradeOffer(packet);

    public int UserId { get; set; }
    public List<TradeItem> Items { get; set; }
    IReadOnlyList<ITradeItem> ITradeOffer.Items => Items;
    public int FurniCount { get; set; }
    public int CreditCount { get; set; }

    public TradeOffer()
    {
        Items = new List<TradeItem>();
    }

    protected TradeOffer(IReadOnlyPacket packet)
        : this()
    {
        UserId = packet.ReadInt();
        int n = packet.ReadInt();
        for (int i = 0; i < n; i++)
            Items.Add(TradeItem.Parse(packet));
        FurniCount = packet.ReadInt();
        CreditCount = packet.ReadInt();
    }
}
