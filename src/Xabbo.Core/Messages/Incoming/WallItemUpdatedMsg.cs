using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a wall item is updated in the room.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>.
/// </summary>
/// <param name="Item">The updated wall item.</param>
public sealed record WallItemUpdatedMsg(WallItem Item) : IMessage<WallItemUpdatedMsg>
{
    static Identifier IMessage<WallItemUpdatedMsg>.Identifier => In.ItemUpdate;
    static WallItemUpdatedMsg IParser<WallItemUpdatedMsg>.Parse(in PacketReader p) => new(p.Parse<WallItem>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Item);
}
