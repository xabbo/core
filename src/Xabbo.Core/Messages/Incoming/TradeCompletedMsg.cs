using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a trade completes successfully.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.TradingCompleted"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.TRADE_COMPLETED_2"/></item>
/// </list>
/// </summary>
public sealed record TradeCompletedMsg : IMessage<TradeCompletedMsg>
{
    static Identifier IMessage<TradeCompletedMsg>.Identifier => In.TradingCompleted;
    static TradeCompletedMsg IParser<TradeCompletedMsg>.Parse(in PacketReader p) => new();
    void IComposer.Compose(in PacketWriter p) { }
}