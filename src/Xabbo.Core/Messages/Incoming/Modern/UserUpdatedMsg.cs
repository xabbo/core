using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

/// <summary>
/// Sent when an avatar's figure, motto or achievement score updates.
/// </summary>
public sealed record AvatarUpdatedMsg(
    int Index, string Figure, Gender Gender, string Motto, int AchievementScore
)
    : IMessage<AvatarUpdatedMsg>
{
    static bool IMessage<AvatarUpdatedMsg>.UseTargetedIdentifiers => true;
    static Identifier IMessage<AvatarUpdatedMsg>.Identifier => In.UserUpdate;

    static AvatarUpdatedMsg IParser<AvatarUpdatedMsg>.Parse(in PacketReader p) => new(
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