using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when a wall item is updated.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.ItemUpdate"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.UPDATEITEM"/></item>
/// </list>
/// </summary>
/// <param name="Item">The updated wall item.</param>
public sealed record WallItemUpdatedMsg(WallItem Item) : IMessage<WallItemUpdatedMsg>
{
    static Identifier IMessage<WallItemUpdatedMsg>.Identifier => In.ItemUpdate;
    static WallItemUpdatedMsg IParser<WallItemUpdatedMsg>.Parse(in PacketReader p) => new(p.Parse<WallItem>());
    void IComposer.Compose(in PacketWriter p) => p.Compose(Item);
}
