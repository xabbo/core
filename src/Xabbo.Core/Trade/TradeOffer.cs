using System.Collections.Generic;

using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class TradeOffer : ITradeOffer, IParserComposer<TradeOffer>
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
        if (p.Client == ClientType.Shockwave)
        {
            UserId = -1;
            UserName = p.ReadString();
            Accepted = p.ReadBool();
        }
        else
        {
            UserId = p.ReadId();
        }

        Items = [..p.ParseArray<TradeItem>()];

        if (p.Client != ClientType.Shockwave)
        {
            FurniCount = p.ReadInt();
            CreditCount = p.ReadInt();
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client == ClientType.Shockwave)
        {
            p.WriteString(UserName ?? "");
            p.WriteBool(Accepted);
        }
        else
        {
            p.WriteId(UserId);
        }

        p.ComposeArray(Items);

        if (p.Client != ClientType.Shockwave)
        {
            p.WriteInt(FurniCount);
            p.WriteInt(CreditCount);
        }
    }

    static TradeOffer IParser<TradeOffer>.Parse(in PacketReader p) => new(in p);
}
