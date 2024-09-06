using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record WallItemUpdatedMsg(WallItem Item) : IMessage<WallItemUpdatedMsg>
{
    static Identifier IMessage<WallItemUpdatedMsg>.Identifier => In.ItemUpdate;
    static WallItemUpdatedMsg IParser<WallItemUpdatedMsg>.Parse(in PacketReader p) => new(p.Parse<WallItem>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Item);
}
