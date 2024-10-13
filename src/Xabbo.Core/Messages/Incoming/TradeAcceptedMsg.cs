using System;
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a user accepts or unaccepts at trade.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.TradingAccept"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.TRADE_ACCEPT"/></item>
/// </list>
/// </summary>
/// <param name="UserId">The ID of the user who accepted or unaccepted. Used on modern clients.</param>
/// <param name="UserName">The name of the user who accepted or unaccepted. Used on the Shockwave client.</param>
/// <param name="Accepted">Whether the user accepted.</param>
public sealed record TradeAcceptedMsg(Id? UserId, string? UserName, bool Accepted) : IMessage<TradeAcceptedMsg>
{
    static Identifier IMessage<TradeAcceptedMsg>.Identifier => In.TradingAccept;

    static TradeAcceptedMsg IParser<TradeAcceptedMsg>.Parse(in PacketReader p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            string packetContent = p.ReadContent();
            if (packetContent.Split('/') is not [ string userName, string accepted ])
                throw new FormatException($"Invalid packet content format when parsing {nameof(TradeAcceptedMsg)}: '{packetContent}'.");
            return new(null, userName, accepted.Equals("true", StringComparison.OrdinalIgnoreCase));
        }
        else
        {
            return new(p.ReadId(), null, p.ReadInt() != 0);
        }
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            if (UserName is null)
                throw new Exception($"{nameof(UserName)} is required on the Shockwave client.");
            p.WriteContent($"{UserName}/{(Accepted ? "true" : "false")}");
        }
        else
        {
            if (UserId is not { } userId)
                throw new Exception($"{nameof(UserId)} is required on the {p.Client} client.");
            p.WriteId(userId);
            p.WriteInt(Accepted ? 1 : 0);
        }
    }
}
