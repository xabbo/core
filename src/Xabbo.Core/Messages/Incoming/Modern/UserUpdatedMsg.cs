using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Received when an avatar's figure, motto or achievement score updates.
/// </summary>
public sealed record UserUpdatedMsg(
    int Index, string Figure, Gender Gender, string Motto, int AchievementScore
)
    : IMessage<UserUpdatedMsg>
{
    static bool IMessage<UserUpdatedMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<UserUpdatedMsg>.Identifier => In.UserUpdate;

    static UserUpdatedMsg IParser<UserUpdatedMsg>.Parse(in PacketReader p) => new(
        Index: p.ReadInt(),
        Figure: p.ReadString(),
        Gender: H.ToGender(p.ReadString()),
        Motto: p.ReadString(),
        AchievementScore: p.ReadInt()
    );

    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(Index);
        p.WriteString(Figure);
        p.WriteString(Gender.ToClientString().ToLower());
        p.WriteString(Motto);
        p.WriteInt(AchievementScore);
    }
}
