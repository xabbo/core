using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class MarketplaceTradeInfo : IMarketplaceTradeInfo, IComposer, IParser<MarketplaceTradeInfo>
{
    public int DayOffset { get; set; }
    public int AverageSalePrice { get; set; }
    public int TradeVolume { get; set; }

    public MarketplaceTradeInfo() { }

    private MarketplaceTradeInfo(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        DayOffset = p.Read<int>();
        AverageSalePrice = p.Read<int>();
        TradeVolume = p.Read<int>();
    }

    public void Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);

        p.Write(DayOffset);
        p.Write(AverageSalePrice);
        p.Write(TradeVolume);
    }

    public static MarketplaceTradeInfo Parse(in PacketReader p) => new(in p);
}
