using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming.Modern;

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
