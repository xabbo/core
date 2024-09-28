using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when an avatar's figure, motto or achievement score updates.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.UserChange"/></item>
/// </list>
/// </summary>
public sealed record UserChangedMsg(
    int Index, string Figure, Gender Gender, string Motto, int AchievementScore
)
    : IMessage<UserChangedMsg>
{
    static ClientType IMessage<UserChangedMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<UserChangedMsg>.Identifier => In.UserChange;

    static UserChangedMsg IParser<UserChangedMsg>.Parse(in PacketReader p) => new(
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
