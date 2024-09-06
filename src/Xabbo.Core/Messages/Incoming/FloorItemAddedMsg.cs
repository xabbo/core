using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

public sealed record FloorItemAddedMsg(FloorItem Item) : IMessage<FloorItemAddedMsg>
{
    static Identifier IMessage<FloorItemAddedMsg>.Identifier => In.ObjectAdd;
    static FloorItemAddedMsg IParser<FloorItemAddedMsg>.Parse(in PacketReader p) => new(p.Parse<FloorItem>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Item);
}