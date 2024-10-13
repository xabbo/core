using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a trade is opened.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.TradingOpen"/></item>
/// </list>
/// </summary>
/// <param name="TraderId">The ID of the user who initiated the trade.</param>
/// <param name="TraderCanTrade">Whether the trader can trade.</param>
/// <param name="TradeeId">The ID of the user who received the trade.</param>
/// <param name="TradeeCanTrade">Whether the tradee can trade.</param>
public sealed record TradeOpenedMsg(
    Id TraderId, bool TraderCanTrade, Id TradeeId, bool TradeeCanTrade
)
    : IMessage<TradeOpenedMsg>
{
    static ClientType IMessage<TradeOpenedMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<TradeOpenedMsg>.Identifier => In.TradingOpen;

    static TradeOpenedMsg IParser<TradeOpenedMsg>.Parse(in PacketReader p) => new(
        p.ReadId(), p.ReadInt() == 1,
        p.ReadId(), p.ReadInt() == 1
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteId(TraderId);
        p.WriteInt(TraderCanTrade ? 1 : 0);
        p.WriteId(TradeeId);
        p.WriteInt(TradeeCanTrade ? 1 : 0);
    }
}
