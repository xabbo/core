using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record FloorItemUpdatedMsg(FloorItem Item) : IMessage<FloorItemUpdatedMsg>
{
    static Identifier IMessage<FloorItemUpdatedMsg>.Identifier => In.ObjectUpdate;
    static FloorItemUpdatedMsg IParser<FloorItemUpdatedMsg>.Parse(in PacketReader p) => new(p.Parse<FloorItem>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Item);
}
