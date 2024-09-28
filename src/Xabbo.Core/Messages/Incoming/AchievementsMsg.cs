using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when achievements are loaded.
/// <para/>
/// Response for <see cref="Outgoing.GetAchievementsMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers: <see cref="In.Achievements"/>
/// </summary>
public sealed record AchievementsMsg(Achievements Achievements) : IMessage<AchievementsMsg>
{
    static ClientType IMessage<AchievementsMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<AchievementsMsg>.Identifier => In.Achievements;

    static AchievementsMsg IParser<AchievementsMsg>.Parse(in PacketReader p) => new(p.Parse<Achievements>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Achievements);
}
