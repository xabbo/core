using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when the user's activity points are updated.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers: <see cref="In.HabboActivityPointNotification"/>
/// </summary>
public sealed record ActivityPointUpdatedMsg(ActivityPointType Type, int Amount, int Change)
    : IMessage<ActivityPointUpdatedMsg>
{
    static ClientType IMessage<ActivityPointUpdatedMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<ActivityPointUpdatedMsg>.Identifier => In.HabboActivityPointNotification;

    static ActivityPointUpdatedMsg IParser<ActivityPointUpdatedMsg>.Parse(in PacketReader p) => new(
        Amount: p.ReadInt(),
        Change: p.ReadInt(),
        Type: (ActivityPointType)p.ReadInt()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Amount);
        p.WriteInt(Change);
        p.WriteInt((int)Type);
    }
}
