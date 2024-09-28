using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing;

/// <summary>
/// Sent when updating the user's motto.
/// <para/>
/// Supported clients:
/// <list type="bullet">
/// <item><see cref="ClientType.Modern"/></item>
/// <item><see cref="ClientType.Origins"/> (Send only: translates to <see cref="UpdateProfileMsg"/></item>
/// </list>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="Out.ChangeMotto"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.Out.UPDATE"/></item>
/// </list>
/// </summary>
public sealed record UpdateMottoMsg(string Motto) : IMessage<UpdateMottoMsg>
{
    static bool IMessage<UpdateMottoMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<UpdateMottoMsg>.Identifier => Out.ChangeMotto;
    Identifier IMessage.GetIdentifier(Xabbo.ClientType client) => client switch
    {
        ClientType.Shockwave => Xabbo.Messages.Shockwave.Out.UPDATE,
        _ => Out.ChangeMotto
    };

    static UpdateMottoMsg IParser<UpdateMottoMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIf(p.Client, ClientType.Shockwave);
        return new(p.ReadString());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        if (p.Client is ClientType.Shockwave)
        {
            p.Compose(new UpdateProfileMsg { Motto = Motto });
        }
        else
        {
            p.WriteString(Motto);
        }
    }
}
