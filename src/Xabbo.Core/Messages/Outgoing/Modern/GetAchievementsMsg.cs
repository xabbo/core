using Xabbo.Messages;
using Xabbo.Messages.Flash;
using Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Messages.Outgoing.Modern;

/// <summary>
/// Sent when requesting the user's achievements.
/// <para/>
/// Request for <see cref="AchievementsMsg"/>.
/// </summary>
public sealed record class GetAchievementsMsg() : IRequestMessage<GetAchievementsMsg, AchievementsMsg, Achievements>
{
    static ClientType IMessage<GetAchievementsMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<GetAchievementsMsg>.Identifier => Out.GetAchievements;
    Achievements IResponseData<AchievementsMsg, Achievements>.GetData(AchievementsMsg msg) => msg.Achievements;
    static GetAchievementsMsg IParser<GetAchievementsMsg>.Parse(in PacketReader p) => new();
    void IComposer.Compose(in PacketWriter p) { }
}
