using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Outgoing.Modern;

public sealed record GetBadgesMsg : IMessage<GetBadgesMsg>
{
    static Identifier IMessage<GetBadgesMsg>.Identifier => Out.GetBadges;
    static GetBadgesMsg IParser<GetBadgesMsg>.Parse(in PacketReader p) => new();
    void IComposer.Compose(in PacketWriter p) { }
}
