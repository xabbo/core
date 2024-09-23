
using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Received when an achievement is updated.
/// </summary>
public sealed record AchievementMsg(Achievement Achievement) : IMessage<AchievementMsg>
{
    static ClientType IMessage<AchievementMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<AchievementMsg>.Identifier => In.Achievement;

    static AchievementMsg IParser<AchievementMsg>.Parse(in PacketReader p) => new(p.Parse<Achievement>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Achievement);
}
