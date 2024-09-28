using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received after requesting the user's list of badges.
/// <para/>
/// A sequence of fragments are received in response to <see cref="Outgoing.GetBadgesMsg"/>.
/// <para/>
/// Supported clients: <see cref="ClientType.Modern"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.Badges"/></item>
/// </list>
/// </summary>
/// <param name="Badges">The list of badges in the fragment.</param>
/// <param name="FragmentIndex">The sequence index of the fragment.</param>
/// <param name="TotalFragments">The total number of fragments in the sequence.</param>
public sealed record BadgeFragmentMsg(Badge[] Badges, int FragmentIndex = 0, int TotalFragments = 1)
    : IMessage<BadgeFragmentMsg>
{
    static ClientType IMessage<BadgeFragmentMsg>.SupportedClients => ClientType.Modern;
    static Identifier IMessage<BadgeFragmentMsg>.Identifier => In.Badges;
    static BadgeFragmentMsg IParser<BadgeFragmentMsg>.Parse(in PacketReader p) => new(
        TotalFragments: p.ReadInt(),
        FragmentIndex: p.ReadInt(),
        Badges: p.ParseArray<Badge>()
    );
    void IComposer.Compose(in PacketWriter p)
    {
        p.WriteInt(TotalFragments);
        p.WriteInt(FragmentIndex);
        p.ComposeArray(Badges);
    }
}
