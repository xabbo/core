using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when activity points are loaded.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers: <see cref="In.ActivityPoints"/>
/// </summary>
public sealed record ActivityPointsMsg(ActivityPoints ActivityPoints) : IMessage<ActivityPointsMsg>
{
    static ClientType IMessage<ActivityPointsMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<ActivityPointsMsg>.Identifier => In.ActivityPoints;

    static ActivityPointsMsg IParser<ActivityPointsMsg>.Parse(in PacketReader p) => new(p.Parse<ActivityPoints>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(ActivityPoints);
}
