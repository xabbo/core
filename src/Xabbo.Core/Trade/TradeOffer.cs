using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class TradeOffer : ITradeOffer, IComposer, IParser<TradeOffer>
{
    public Id UserId { get; set; }
    public string? UserName { get; set; }
    public bool Accepted { get; set; }
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
        UnsupportedClientException.ThrowIfUnknown(p.Client);

        if (p.Client == ClientType.Shockwave)
        {
            UserId = -1;
            UserName = p.Read<string>();
            Accepted = p.Read<bool>();
        }
        else
        {
            UserId = p.Read<Id>();
        }

        Items = [..p.ParseArray<TradeItem>()];

        if (p.Client != ClientType.Shockwave)
        {
            FurniCount = p.Read<int>();
            CreditCount = p.Read<int>();
        }
    }

    public void Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfUnknown(p.Client);

        if (p.Client == ClientType.Shockwave)
        {
            p.Write(UserName);
            p.Write(Accepted);
        }
        else
        {
            p.Write(UserId);
        }

        p.Write(Items);

        if (p.Client != ClientType.Shockwave)
        {
            p.Write(FurniCount);
            p.Write(CreditCount);
        }
    }

    public static TradeOffer Parse(in PacketReader p) => new(in p);
}
