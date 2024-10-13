using System;

using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a trade is closed.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.TradingClose"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.TRADE_CLOSE"/></item>
/// </list>
/// </summary>
/// <param name="UserId">The ID of the user who closed the trade. Used on modern clients.</param>
/// <param name="Reason">The reason that the trade was closed. Used on modern clients.</param>
public sealed record TradeClosedMsg(Id? UserId = null, int? Reason = null) : IMessage<TradeClosedMsg>
{
    static Identifier IMessage<TradeClosedMsg>.Identifier => In.TradingClose;

    static TradeClosedMsg IParser<TradeClosedMsg>.Parse(in PacketReader p) => p.Client switch
    {
        ClientType.Shockwave => new(),
        not ClientType.Shockwave => new(p.ReadId(), p.ReadInt())
    };

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is not ClientType.Shockwave)
        {
            if (UserId is not { } userId)
                throw new Exception($"{nameof(UserId)} is required on {p.Client}.");
            if (Reason is not { } reason)
                throw new Exception($"{nameof(Reason)} is required on {p.Client}.");
            p.WriteId(userId);
            p.WriteInt(reason);
        }
    }
}