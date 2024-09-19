using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Received when achievements are loaded.
/// <para/>
/// Supported on:
/// <list type="bullet">
/// <item>Flash</item>
/// </list>
/// </summary>
public sealed record AchievementsMsg(Achievements Achievements) : IMessage<AchievementsMsg>
{
    static bool IMessage<AchievementsMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<AchievementsMsg>.Identifier => In.Achievements;

    static AchievementsMsg IParser<AchievementsMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfOrigins(p.Client);
        return new AchievementsMsg(p.Parse<Achievements>());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfOrigins(p.Client);
        p.Compose(Achievements);
    }
}
