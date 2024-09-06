using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record WallItemAddedMsg(WallItem Item) : IMessage<WallItemAddedMsg>
{
    static Identifier IMessage<WallItemAddedMsg>.Identifier => In.ItemAdd;
    static WallItemAddedMsg IParser<WallItemAddedMsg>.Parse(in PacketReader p) => new(p.Parse<WallItem>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Item);
}