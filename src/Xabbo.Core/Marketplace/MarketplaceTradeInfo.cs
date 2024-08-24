using Xabbo.Messages;

namespace Xabbo.Core;

public sealed class MarketplaceTradeInfo : IMarketplaceTradeInfo, IComposer, IParser<MarketplaceTradeInfo>
{
    public int DayOffset { get; set; }
    public int AverageSalePrice { get; set; }
    public int TradeVolume { get; set; }

    public MarketplaceTradeInfo() { }

    private MarketplaceTradeInfo(in PacketReader packet)
    {
        DayOffset = packet.Read<int>();
        AverageSalePrice = packet.Read<int>();
        TradeVolume = packet.Read<int>();
    }

    public static MarketplaceTradeInfo Parse(in PacketReader packet) => new(in packet);

    public void Compose(in PacketWriter p)
    {
        p.Write(DayOffset);
        p.Write(AverageSalePrice);
        p.Write(TradeVolume);
    }

}
