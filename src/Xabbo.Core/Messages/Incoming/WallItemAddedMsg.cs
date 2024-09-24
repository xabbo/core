using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record WallItemAddedMsg(WallItem Item) : IMessage<WallItemAddedMsg>
{
    static Identifier IMessage<WallItemAddedMsg>.Identifier => In.ItemAdd;
    static WallItemAddedMsg IParser<WallItemAddedMsg>.Parse(in PacketReader p)
    {
        var item = p.Parse<WallItem>();
        if (p.Client is not ClientType.Shockwave)
            item.OwnerName = p.ReadString();
        return new(item);
    }
    void IComposer.Compose(in PacketWriter p)
    {
        p.Compose(Item);
        if (p.Client is not ClientType.Shockwave)
            p.WriteString(Item.OwnerName);
    }
}
