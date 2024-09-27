using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a floor item is updated in the room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>.
/// </summary>
/// <param name="Item">The updated floor item.</param>
public sealed record FloorItemUpdatedMsg(FloorItem Item) : IMessage<FloorItemUpdatedMsg>
{
    static Identifier IMessage<FloorItemUpdatedMsg>.Identifier => In.ObjectUpdate;
    static FloorItemUpdatedMsg IParser<FloorItemUpdatedMsg>.Parse(in PacketReader p) => new(p.Parse<FloorItem>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Item);
}
