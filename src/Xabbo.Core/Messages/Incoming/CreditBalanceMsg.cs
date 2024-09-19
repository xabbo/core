using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when the user's credit balance changes or is requested.
/// </summary>
public sealed record CreditBalanceMsg(int Credits) : IMessage<CreditBalanceMsg>
{
    static Identifier IMessage<CreditBalanceMsg>.Identifier => In.CreditBalance;

    static CreditBalanceMsg IParser<CreditBalanceMsg>.Parse(in PacketReader p)
        => new((int)(FloatAsString)p.ReadString());

    void IComposer.Compose(in PacketWriter p) => p.WriteString(Credits.ToString());
}
