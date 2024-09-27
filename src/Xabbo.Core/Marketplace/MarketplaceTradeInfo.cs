using Xabbo.Messages;

namespace Xabbo.Core;

/// <inheritdoc cref="IMarketplaceTradeInfo"/>
public sealed class MarketplaceTradeInfo : IMarketplaceTradeInfo, IParserComposer<MarketplaceTradeInfo>
{
    public int DayOffset { get; set; }
    public int AverageSalePrice { get; set; }
    public int TradeVolume { get; set; }

    public MarketplaceTradeInfo() { }

    private MarketplaceTradeInfo(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        DayOffset = p.ReadInt();
        AverageSalePrice = p.ReadInt();
        TradeVolume = p.ReadInt();
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.WriteInt(DayOffset);
        p.WriteInt(AverageSalePrice);
        p.WriteInt(TradeVolume);
    }

    static MarketplaceTradeInfo IParser<MarketplaceTradeInfo>.Parse(in PacketReader p) => new(in p);
}
