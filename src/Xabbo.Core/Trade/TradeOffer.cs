using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class TradeOffer : ITradeOffer, IComposer, IParser<TradeOffer>
{
    public static TradeOffer Parse(in PacketReader packet) => new(in packet);

    public Id UserId { get; set; }
    public List<TradeItem> Items { get; set; }
    IReadOnlyList<ITradeItem> ITradeOffer.Items => Items;
    public int FurniCount { get; set; }
    public int CreditCount { get; set; }

    public TradeOffer()
    {
        Items = [];
    }

    private TradeOffer(in PacketReader p) : this()
    {
        UserId = p.Read<Id>();
        int n = p.Read<Length>();
        for (int i = 0; i < n; i++)
            Items.Add(p.Parse<TradeItem>());
        FurniCount = p.Read<int>();
        CreditCount = p.Read<int>();
    }

    public void Compose(in PacketWriter p)
    {
        p.Write(UserId);
        p.Write<Length>(Items.Count);
        foreach (var item in Items)
            p.Write(item);
        p.Write(FurniCount);
        p.Write(CreditCount);
    }


}
