
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Received when an achievement is updated.
/// <para/>
/// Supported on:
/// <list type="bullet">
/// <item>Flash</item>
/// </list>
/// </summary>
public sealed record AchievementMsg(Achievement Achievement) : IMessage<AchievementMsg>
{
    static bool IMessage<AchievementMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<AchievementMsg>.Identifier => In.Achievement;

    static AchievementMsg IParser<AchievementMsg>.Parse(in PacketReader p)
    {
        UnsupportedClientException.ThrowIfOrigins(p.Client);
        return new AchievementMsg(p.Parse<Achievement>());
    }

    void IComposer.Compose(in PacketWriter p)
    {
        UnsupportedClientException.ThrowIfOrigins(p.Client);
        p.Compose(Achievement);
    }
}
