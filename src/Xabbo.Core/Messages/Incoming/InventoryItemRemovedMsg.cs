using Xabbo.Messages;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Messages.Incoming;

/// <summary>
/// Received when an item is removed from the user's inventory.
/// <para/>
/// Supported clients: <see cref="ClientType.All"/>
/// <para/>
/// Identifiers:
/// <list type="bullet">
/// <item>Flash: <see cref="In.FurniListRemove"/></item>
/// <item>Shockwave: <see cref="Xabbo.Messages.Shockwave.In.REMOVESTRIPITEM"/></item>
/// </list>
/// </summary>
/// <param name="ItemId">The ID of the item that was removed.</param>
public sealed record InventoryItemRemovedMsg(Id ItemId) : IMessage<InventoryItemRemovedMsg>
{
    static Identifier IMessage<InventoryItemRemovedMsg>.Identifier => In.FurniListRemove;
    static InventoryItemRemovedMsg IParser<InventoryItemRemovedMsg>.Parse(in PacketReader p) => new(p.ReadId());
    void IComposer.Compose(in PacketWriter p) => p.WriteId(ItemId);
}