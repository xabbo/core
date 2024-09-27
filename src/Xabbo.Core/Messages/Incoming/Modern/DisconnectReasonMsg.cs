using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// May be received before the server closes the connection to specify a reason for the disconnection.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>.
/// </summary>
public sealed record DisconnectReasonMsg(DisconnectReason Reason = DisconnectReason.Unknown) : IMessage<DisconnectReasonMsg>
{
    static ClientType IMessage<DisconnectReasonMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<DisconnectReasonMsg>.Identifier => In.DisconnectReason;
    static DisconnectReasonMsg IParser<DisconnectReasonMsg>.Parse(in PacketReader p)
    {
        var reason = DisconnectReason.Unknown;
        if (p.Available >= 4)
            p.ReadInt();
        return new(reason);
    }
    void IComposer.Compose(in PacketWriter p) => p.WriteInt((int)Reason);
}
