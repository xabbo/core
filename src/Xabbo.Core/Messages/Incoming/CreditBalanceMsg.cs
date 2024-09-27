using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when the user's credit balance changes or is explicitly requested.
/// <para/>
/// Response for <see cref="Outgoing.GetCreditBalanceMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>.
/// </summary>
public sealed record CreditBalanceMsg(int Credits) : IMessage<CreditBalanceMsg>
{
    static Identifier IMessage<CreditBalanceMsg>.Identifier => In.CreditBalance;

    static CreditBalanceMsg IParser<CreditBalanceMsg>.Parse(in PacketReader p)
        => new((int)(FloatString)p.ReadString());

    void IComposer.Compose(in PacketWriter p) => p.WriteString(Credits.ToString());
}
