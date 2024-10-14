using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when both users have accepted the trade and the server is now awaiting confirmation
/// from both users.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.TradingConfirmation"/></item>
/// </list>
/// </summary>
public sealed class TradeAwaitingConfirmationMsg : IMessage<TradeAwaitingConfirmationMsg>
{
    static ClientType IMessage<TradeAwaitingConfirmationMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<TradeAwaitingConfirmationMsg>.Identifier => In.TradingConfirmation;
    static TradeAwaitingConfirmationMsg IParser<TradeAwaitingConfirmationMsg>.Parse(in PacketReader p) => new();
    void IComposer.Compose(in PacketWriter p) { }
}