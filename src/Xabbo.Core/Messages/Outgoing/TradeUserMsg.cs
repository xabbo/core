using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when opening a trade with a user.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.OpenTrading"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.TRADE_OPEN"/></item>
/// </list>
/// </summary>
/// <param name="UserIndex">The avatar index of the user to trade.</param>
public sealed record TradeUserMsg(int UserIndex) : IMessage<TradeUserMsg>
{
    static Identifier IMessage<TradeUserMsg>.Identifier => Out.OpenTrading;

    static TradeUserMsg IParser<TradeUserMsg>.Parse(in PacketReader p) => new(p.Client switch
    {
        ClientType.Shockwave => int.Parse(p.ReadContent()),
        not ClientType.Shockwave => p.ReadInt(),
    });

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.WriteContent(UserIndex.ToString());
        }
        else
        {
            p.WriteInt(UserIndex);
        }
    }
}