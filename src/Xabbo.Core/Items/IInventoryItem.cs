using System;
using Xabbo.Messages;

namespace Xabbo.Core;

public interface IInventoryItem : IItem, IComposable
{
    /// <summary>
    /// Gets the ID of the inventory item.
    /// </summary>
    long ItemId { get; }
    /// <summary>
    /// Gets the furni category of the inventory item.
    /// </summary>
    FurniCategory Category { get; }
    /// <summary>
    /// Gets the item data of the inventory item.
    /// </summary>
    IItemData Data { get; }
    /// <summary>
    /// Gets if the inventory item is recyclable.
    /// </summary>
    bool IsRecyclable { get; }
    /// <summary>
    /// Gets if the inventory item is groupable.
    /// </summary>
    bool IsGroupable { get; }
    /// <summary>
    /// Gets if the inventory item is tradeable.
    /// </summary>
    bool IsTradeable { get; }
    /// <summary>
    /// Gets if the inventory item is sellable in the marketplace.
    /// </summary>
    bool IsSellable { get; }
    /// <summary>
    /// Gets the seconds to expiration of the inventory item.
    /// </summary>
    int SecondsToExpiration { get; }
    /// <summary>
    /// Gets if the rent period of the inventory item has started.
    /// </summary>
    bool HasRentPeriodStarted { get; }
    /// <summary>
    /// Gets the room ID that the inventory item is in.
    /// </summary>
    long RoomId { get; }
    string SlotId { get; }
    /// <summary>
    /// Gets the extra state of the inventory item which is used for
    /// consumable state, linked teleporter ID, etc.
    /// </summary>
    long Extra { get; }
}
